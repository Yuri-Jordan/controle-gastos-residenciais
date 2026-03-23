using AutoMapper;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Enums;
using ControleGastosResidenciais.Core.Interfaces;

namespace ControleGastosResidenciais.Infraestrutura.Services
{
    /// <summary>
    /// Serviço para gerenciamento de operações de categoria com lógica de negócio
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly IRepositorio<Categoria> _repositorioCategoria;
        private readonly IMapper _mapper;

        public CategoriaService(IRepositorio<Categoria> repositorioCategoria, IMapper mapper)
        {
            _repositorioCategoria = repositorioCategoria;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoriaDto>> ObterTodasCategoriasAsync()
        {
            var categorias = await _repositorioCategoria.ObterTodosAsync();
            var categoriaDtos = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);

            // Definir propriedade ExibirFinalidade
            foreach (var dto in categoriaDtos)
            {
                dto.ExibirFinalidade = ObterDescricaoFinalidade(dto.Finalidade);
            }

            return categoriaDtos;
        }

        public async Task<CategoriaDto> CriarCategoriaAsync(CriarCategoriaDto criarCategoriaDto)
        {
            // Validar descrição
            if (string.IsNullOrWhiteSpace(criarCategoriaDto.Descricao))
                throw new ArgumentException("A descrição da categoria é obrigatória");

            // Validar comprimento da descrição
            if (criarCategoriaDto.Descricao.Length > 400)
                throw new ArgumentException("A descrição da categoria não pode exceder 400 caracteres");

            // Verificar se já existe uma categoria com a mesma descrição
            var categoriaExistente = await _repositorioCategoria.BuscarAsync(c =>
                c.Descricao.ToLower() == criarCategoriaDto.Descricao.ToLower());
            if (categoriaExistente.Any())
                throw new InvalidOperationException("Já existe uma categoria com esta descrição");

            var categoria = _mapper.Map<Categoria>(criarCategoriaDto);
            var categoriaCriada = await _repositorioCategoria.AdicionarAsync(categoria);

            var dto = _mapper.Map<CategoriaDto>(categoriaCriada);
            dto.ExibirFinalidade = ObterDescricaoFinalidade(dto.Finalidade);

            return dto;
        }

        private string ObterDescricaoFinalidade(FinalidadeCategoria finalidade)
        {
            return finalidade switch
            {
                FinalidadeCategoria.Despesa => "Despesa",
                FinalidadeCategoria.Receita => "Receita",
                FinalidadeCategoria.Ambos => "Ambos",
                _ => "Desconhecido"
            };
        }
    }
}