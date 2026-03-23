using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Enums;
using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Data;

namespace ControleGastosResidenciais.Infraestrutura.Services
{
    /// <summary>
    /// Serviço para gerenciamento de transações com validação de regras de negócio
    /// </summary>
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepositorio _transacaoRepositorio;
        private readonly IPessoaRepositorio _pessoaRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IMapper _mapper;

        public TransacaoService(
            ITransacaoRepositorio transacaoRepositorio,
            IPessoaRepositorio pessoaRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IMapper mapper)
        {
            _transacaoRepositorio = transacaoRepositorio;
            _pessoaRepositorio = pessoaRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransacaoDto>> ObterTodasTransacoesAsync()
        {
            var transacoes = await _transacaoRepositorio.ObterTodasComIncludesAsync();
            return _mapper.Map<IEnumerable<TransacaoDto>>(transacoes);
        }

        public async Task<TransacaoDto?> ObterTransacaoPorIdAsync(Guid id)
        {
            var transacao = await _transacaoRepositorio.ObterPorIdComIncludesAsync(id);
            return transacao == null ? null : _mapper.Map<TransacaoDto>(transacao);
        }

        public async Task<TransacaoDto> CriarTransacaoAsync(CriarTransacaoDto criarTransacaoDto)
        {
            // Validar valor (deve ser positivo)
            if (criarTransacaoDto.Valor <= 0)
                throw new ArgumentException("O valor deve ser maior que 0");
            
            // Validar descrição
            if (string.IsNullOrWhiteSpace(criarTransacaoDto.Descricao))
                throw new ArgumentException("A descrição é obrigatória");
            
            if (criarTransacaoDto.Descricao.Length > 400)
                throw new ArgumentException("A descrição não pode exceder 400 caracteres");
            
            // Obter pessoa para verificar idade
            var pessoa = await _pessoaRepositorio.ObterPorIdAsync(criarTransacaoDto.PessoaId);
            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {criarTransacaoDto.PessoaId} não encontrada");
            
            // Regra de negócio: menores de idade só podem criar despesas
            if (pessoa.Idade < 18 && criarTransacaoDto.Tipo == TipoTransacao.Receita)
                throw new InvalidOperationException("Menores de idade (idade < 18) só podem criar transações de despesa");
            
            // Obter categoria para validar compatibilidade
            var categoria = await _categoriaRepositorio.ObterPorIdAsync(criarTransacaoDto.CategoriaId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {criarTransacaoDto.CategoriaId} não encontrada");
            
            // Regra de negócio: categoria deve ser compatível com o tipo de transação
            bool categoriaCompativel = criarTransacaoDto.Tipo switch
            {
                TipoTransacao.Despesa => categoria.Finalidade == FinalidadeCategoria.Despesa || categoria.Finalidade == FinalidadeCategoria.Ambos,
                TipoTransacao.Receita => categoria.Finalidade == FinalidadeCategoria.Receita || categoria.Finalidade == FinalidadeCategoria.Ambos,
                _ => false
            };
            
            if (!categoriaCompativel)
                throw new InvalidOperationException($"Categoria '{categoria.Descricao}' não pode ser usada para transações de {criarTransacaoDto.Tipo}");
            
            // Criar transação
            var transacao = _mapper.Map<Transacao>(criarTransacaoDto);
            transacao.CriadoEm = DateTime.UtcNow;
            
            await _transacaoRepositorio.AdicionarAsync(transacao);
            
            // Recarregar com includes para resposta
            var transacaoCriada = await _transacaoRepositorio.ObterPorIdComIncludesAsync(transacao.Id);
            
            return _mapper.Map<TransacaoDto>(transacaoCriada);
        }

        public async Task ExcluirTransacaoAsync(Guid id)
        {
            var transacao = await _transacaoRepositorio.ObterPorIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada");
            
            await _transacaoRepositorio.ExcluirAsync(transacao);
        }

        public async Task<IEnumerable<TransacaoDto>> ObterTransacoesPorPessoaAsync(Guid pessoaId)
        {
            var transacoes = await _transacaoRepositorio.ObterPorPessoaComIncludesAsync(pessoaId);
            return _mapper.Map<IEnumerable<TransacaoDto>>(transacoes);
        }

        public async Task<IEnumerable<TransacaoDto>> ObterTransacoesPorCategoriaAsync(Guid categoriaId)
        {
            var transacoes = await _transacaoRepositorio.ObterPorCategoriaComIncludesAsync(categoriaId);
            return _mapper.Map<IEnumerable<TransacaoDto>>(transacoes);
        }
    }
}