using System.Linq.Expressions;
using AutoMapper;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Services;
using FluentAssertions;
using Moq;

namespace ControleGastosResidenciais.Testes.UnitTests.Services
{
    public class CategoriaServiceTests
    {
        private readonly Mock<IRepositorio<Categoria>> _mockRepositorio;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoriaService _categoriaService;

        public CategoriaServiceTests()
        {
            _mockRepositorio = new Mock<IRepositorio<Categoria>>();
            _mockMapper = new Mock<IMapper>();
            _categoriaService = new CategoriaService(_mockRepositorio.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task ObterTodasCategoriasAsync_DeveRetornarListaDeCategoriasComExibirFinalidade()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = Guid.NewGuid(), Descricao = "Alimentação", Finalidade = Core.Enums.FinalidadeCategoria.Despesa },
                new Categoria { Id = Guid.NewGuid(), Descricao = "Salário", Finalidade = Core.Enums.FinalidadeCategoria.Receita }
            };

            var categoriaDtos = new List<CategoriaDto>
            {
                new CategoriaDto { Id = categorias[0].Id, Descricao = "Alimentação", Finalidade = Core.Enums.FinalidadeCategoria.Despesa },
                new CategoriaDto { Id = categorias[1].Id, Descricao = "Salário", Finalidade = Core.Enums.FinalidadeCategoria.Receita }
            };

            _mockRepositorio.Setup(r => r.ObterTodosAsync()).ReturnsAsync(categorias);
            _mockMapper.Setup(m => m.Map<IEnumerable<CategoriaDto>>(categorias)).Returns(categoriaDtos);

            // Act
            var resultado = await _categoriaService.ObterTodasCategoriasAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.First().ExibirFinalidade.Should().Be("Despesa");
            resultado.Last().ExibirFinalidade.Should().Be("Receita");
        }

        [Fact]
        public async Task CriarCategoriaAsync_ComDadosValidos_DeveRetornarCategoriaDto()
        {
            // Arrange
            var criarCategoriaDto = new CriarCategoriaDto
            {
                Descricao = "Transporte",
                Finalidade = Core.Enums.FinalidadeCategoria.Despesa
            };

            var categoria = new Categoria
            {
                Id = Guid.NewGuid(),
                Descricao = "Transporte",
                Finalidade = Core.Enums.FinalidadeCategoria.Despesa
            };

            var categoriaDto = new CategoriaDto
            {
                Id = categoria.Id,
                Descricao = "Transporte",
                Finalidade = Core.Enums.FinalidadeCategoria.Despesa
            };

            _mockMapper.Setup(m => m.Map<Categoria>(criarCategoriaDto)).Returns(categoria);
            _mockRepositorio.Setup(r => r.AdicionarAsync(categoria)).ReturnsAsync(categoria);
            _mockMapper.Setup(m => m.Map<CategoriaDto>(categoria)).Returns(categoriaDto);

            // Act
            var resultado = await _categoriaService.CriarCategoriaAsync(criarCategoriaDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Descricao.Should().Be("Transporte");
            resultado.ExibirFinalidade.Should().Be("Despesa");
        }

        [Fact]
        public async Task CriarCategoriaAsync_ComDescricaoVazia_DeveLancarArgumentException()
        {
            // Arrange
            var criarCategoriaDto = new CriarCategoriaDto
            {
                Descricao = "",
                Finalidade = Core.Enums.FinalidadeCategoria.Despesa
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _categoriaService.CriarCategoriaAsync(criarCategoriaDto));
        }

        [Fact]
        public async Task CriarCategoriaAsync_ComDescricaoMaiorQue400Caracteres_DeveLancarArgumentException()
        {
            // Arrange
            var criarCategoriaDto = new CriarCategoriaDto
            {
                Descricao = new string('A', 401),
                Finalidade = Core.Enums.FinalidadeCategoria.Despesa
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _categoriaService.CriarCategoriaAsync(criarCategoriaDto));
        }

        [Fact]
        public async Task CriarCategoriaAsync_ComDescricaoDuplicada_DeveLancarInvalidOperationException()
        {
            // Arrange
            var criarCategoriaDto = new CriarCategoriaDto
            {
                Descricao = "Alimentação",
                Finalidade = Core.Enums.FinalidadeCategoria.Despesa
            };

            var categoriasExistentes = new List<Categoria>
            {
                new Categoria { Descricao = "Alimentação", Finalidade = Core.Enums.FinalidadeCategoria.Despesa }
            };

            _mockRepositorio.Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(categoriasExistentes);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _categoriaService.CriarCategoriaAsync(criarCategoriaDto));
        }
    }
}