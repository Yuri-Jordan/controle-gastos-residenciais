using AutoMapper;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ExpenseControl.Core.Interfaces;

namespace ExpenseControl.Infrastructure.Services
{
    /// <summary>
    /// Serviço para gerenciamento de operações de pessoa com lógica de negócio
    /// </summary>
    public class ServicoPessoa : IPessoaService
    {
        private readonly IRepositorio<Pessoa> _repositorioPessoa;
        private readonly IMapper _mapper;

        public ServicoPessoa(IRepositorio<Pessoa> repositorioPessoa, IMapper mapper)
        {
            _repositorioPessoa = repositorioPessoa;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PessoaDto>> ObterTodasPessoasAsync()
        {
            var pessoas = await _repositorioPessoa.ObterTodosAsync();
            var pessoaDtos = _mapper.Map<IEnumerable<PessoaDto>>(pessoas);
            
            // Definir propriedade EhMenorDeIdade
            foreach (var dto in pessoaDtos)
            {
                dto.EhMenorDeIdade = dto.Idade < 18;
            }
            
            return pessoaDtos;
        }

        public async Task<PessoaDto?> ObterPessoaPorIdAsync(Guid id)
        {
            var pessoa = await _repositorioPessoa.ObterPorIdAsync(id);
            if (pessoa == null) return null;
            
            var dto = _mapper.Map<PessoaDto>(pessoa);
            dto.EhMenorDeIdade = dto.Idade < 18;
            
            return dto;
        }

        public async Task<PessoaDto> CriarPessoaAsync(CriarPessoaDto criarPessoaDto)
        {
            // Validar idade (deve ser >= 0)
            if (criarPessoaDto.Idade < 0)
                throw new ArgumentException("A idade deve ser maior ou igual a 0");
            
            // Validar nome
            if (string.IsNullOrWhiteSpace(criarPessoaDto.Nome))
                throw new ArgumentException("O nome é obrigatório");
            
            var pessoa = _mapper.Map<Pessoa>(criarPessoaDto);
            var pessoaCriada = await _repositorioPessoa.AdicionarAsync(pessoa);
            
            var dto = _mapper.Map<PessoaDto>(pessoaCriada);
            dto.EhMenorDeIdade = dto.Idade < 18;
            
            return dto;
        }

        public async Task<PessoaDto> AtualizarPessoaAsync(Guid id, AtualizarPessoaDto atualizarPessoaDto)
        {
            var pessoa = await _repositorioPessoa.ObterPorIdAsync(id);
            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada");
            
            // Validar idade
            if (atualizarPessoaDto.Idade < 0)
                throw new ArgumentException("A idade deve ser maior ou igual a 0");
            
            // Validar nome
            if (string.IsNullOrWhiteSpace(atualizarPessoaDto.Nome))
                throw new ArgumentException("O nome é obrigatório");
            
            _mapper.Map(atualizarPessoaDto, pessoa);
            await _repositorioPessoa.AtualizarAsync(pessoa);
            
            var dto = _mapper.Map<PessoaDto>(pessoa);
            dto.EhMenorDeIdade = dto.Idade < 18;
            
            return dto;
        }

        public async Task ExcluirPessoaAsync(Guid id)
        {
            var pessoa = await _repositorioPessoa.ObterPorIdAsync(id);
            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada");
            
            // Exclusão em cascata irá tratar transações associadas
            await _repositorioPessoa.ExcluirAsync(pessoa);
        }
    }
}