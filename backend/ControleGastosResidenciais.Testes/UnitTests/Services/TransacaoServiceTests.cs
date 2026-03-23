using AutoMapper;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Enums;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Services;
using FluentAssertions;
using Moq;

namespace ControleGastosResidenciais.Testes.UnitTests.Services
{
    public class TransacaoServiceTests
    {
        private readonly Mock<ITransacaoRepositorio> _mockTransacaoRepositorio;
        private readonly Mock<IPessoaRepositorio> _mockPessoaRepositorio;
        private readonly Mock<ICategoriaRepositorio> _mockCategoriaRepositorio;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TransacaoService _transacaoService;

        public TransacaoServiceTests()
        {
            _mockTransacaoRepositorio = new Mock<ITransacaoRepositorio>();
            _mockPessoaRepositorio = new Mock<IPessoaRepositorio>();
            _mockCategoriaRepositorio = new Mock<ICategoriaRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _transacaoService = new TransacaoService(
                _mockTransacaoRepositorio.Object,
                _mockPessoaRepositorio.Object,
                _mockCategoriaRepositorio.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task ObterTodasTransacoesAsync_DeveRetornarListaOrdenadaPorDataDecrescente()
        {
            // Arrange
            var transacoes = new List<Transacao>
            {
                new Transacao { Id = Guid.NewGuid(), Descricao = "Transação antiga", CriadoEm = DateTime.UtcNow.AddDays(-1) },
                new Transacao { Id = Guid.NewGuid(), Descricao = "Transação recente", CriadoEm = DateTime.UtcNow }
            };

            var transacaoDtos = new List<TransacaoDto>
            {
                new TransacaoDto { Id = transacoes[1].Id, Descricao = "Transação recente" },
                new TransacaoDto { Id = transacoes[0].Id, Descricao = "Transação antiga" }
            };

            _mockTransacaoRepositorio.Setup(r => r.ObterTodasComIncludesAsync())
                .ReturnsAsync(transacoes);
            _mockMapper.Setup(m => m.Map<IEnumerable<TransacaoDto>>(transacoes))
                .Returns(transacaoDtos);

            // Act
            var resultado = await _transacaoService.ObterTodasTransacoesAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.First().Descricao.Should().Be("Transação recente");
            resultado.Last().Descricao.Should().Be("Transação antiga");
        }

        [Fact]
        public async Task ObterTransacaoPorIdAsync_ComIdExistente_DeveRetornarTransacaoDto()
        {
            // Arrange
            var transacaoId = Guid.NewGuid();
            var transacao = new Transacao { Id = transacaoId, Descricao = "Compra mercado" };
            var transacaoDto = new TransacaoDto { Id = transacaoId, Descricao = "Compra mercado" };

            _mockTransacaoRepositorio.Setup(r => r.ObterPorIdComIncludesAsync(transacaoId))
                .ReturnsAsync(transacao);
            _mockMapper.Setup(m => m.Map<TransacaoDto>(transacao))
                .Returns(transacaoDto);

            // Act
            var resultado = await _transacaoService.ObterTransacaoPorIdAsync(transacaoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(transacaoId);
            resultado.Descricao.Should().Be("Compra mercado");
        }

        [Fact]
        public async Task ObterTransacaoPorIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var transacaoId = Guid.NewGuid();
            _mockTransacaoRepositorio.Setup(r => r.ObterPorIdComIncludesAsync(transacaoId))
                .ReturnsAsync((Transacao?)null);

            // Act
            var resultado = await _transacaoService.ObterTransacaoPorIdAsync(transacaoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComDadosValidos_DeveRetornarTransacaoDto()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 25 };
            var categoria = new Categoria { Id = categoriaId, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa };

            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "Compra mercado",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa,
                PessoaId = pessoaId,
                CategoriaId = categoriaId
            };

            var transacao = new Transacao
            {
                Id = Guid.NewGuid(),
                Descricao = "Compra mercado",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa,
                PessoaId = pessoaId,
                CategoriaId = categoriaId
            };

            var transacaoDto = new TransacaoDto
            {
                Id = transacao.Id,
                Descricao = "Compra mercado",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa
            };

            _mockPessoaRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockCategoriaRepositorio.Setup(r => r.ObterPorIdAsync(categoriaId)).ReturnsAsync(categoria);
            _mockMapper.Setup(m => m.Map<Transacao>(criarTransacaoDto)).Returns(transacao);
            _mockTransacaoRepositorio.Setup(r => r.AdicionarAsync(transacao)).ReturnsAsync(transacao);
            _mockTransacaoRepositorio.Setup(r => r.ObterPorIdComIncludesAsync(transacao.Id)).ReturnsAsync(transacao);
            _mockMapper.Setup(m => m.Map<TransacaoDto>(transacao)).Returns(transacaoDto);

            // Act
            var resultado = await _transacaoService.CriarTransacaoAsync(criarTransacaoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Descricao.Should().Be("Compra mercado");
            _mockTransacaoRepositorio.Verify(r => r.AdicionarAsync(transacao), Times.Once);
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComValorZero_DeveLancarArgumentException()
        {
            // Arrange
            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "Compra mercado",
                Valor = 0,
                Tipo = TipoTransacao.Despesa,
                PessoaId = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transacaoService.CriarTransacaoAsync(criarTransacaoDto));
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComDescricaoVazia_DeveLancarArgumentException()
        {
            // Arrange
            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa,
                PessoaId = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transacaoService.CriarTransacaoAsync(criarTransacaoDto));
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComPessoaInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "Compra mercado",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa,
                PessoaId = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid()
            };

            _mockPessoaRepositorio.Setup(r => r.ObterPorIdAsync(criarTransacaoDto.PessoaId)).ReturnsAsync((Pessoa?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _transacaoService.CriarTransacaoAsync(criarTransacaoDto));
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComCategoriaInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 25 };

            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "Compra mercado",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa,
                PessoaId = pessoaId,
                CategoriaId = Guid.NewGuid()
            };

            _mockPessoaRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockCategoriaRepositorio.Setup(r => r.ObterPorIdAsync(criarTransacaoDto.CategoriaId)).ReturnsAsync((Categoria?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _transacaoService.CriarTransacaoAsync(criarTransacaoDto));
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComMenorDeIdadeTentandoReceita_DeveLancarInvalidOperationException()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 16 };
            var categoria = new Categoria { Id = categoriaId, Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita };

            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "Salário",
                Valor = 1000.00m,
                Tipo = TipoTransacao.Receita,
                PessoaId = pessoaId,
                CategoriaId = categoriaId
            };

            _mockPessoaRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockCategoriaRepositorio.Setup(r => r.ObterPorIdAsync(categoriaId)).ReturnsAsync(categoria);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _transacaoService.CriarTransacaoAsync(criarTransacaoDto));
        }

        [Fact]
        public async Task CriarTransacaoAsync_ComCategoriaIncompativel_DeveLancarInvalidOperationException()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 25 };
            var categoria = new Categoria { Id = categoriaId, Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita };

            var criarTransacaoDto = new CriarTransacaoDto
            {
                Descricao = "Compra mercado",
                Valor = 150.00m,
                Tipo = TipoTransacao.Despesa,
                PessoaId = pessoaId,
                CategoriaId = categoriaId
            };

            _mockPessoaRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockCategoriaRepositorio.Setup(r => r.ObterPorIdAsync(categoriaId)).ReturnsAsync(categoria);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _transacaoService.CriarTransacaoAsync(criarTransacaoDto));
        }

        [Fact]
        public async Task ObterTransacoesPorPessoaAsync_DeveRetornarTransacoesDaPessoa()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var transacoes = new List<Transacao>
            {
                new Transacao { Id = Guid.NewGuid(), PessoaId = pessoaId, Descricao = "Compra 1" },
                new Transacao { Id = Guid.NewGuid(), PessoaId = pessoaId, Descricao = "Compra 2" }
            };

            var transacaoDtos = transacoes.Select(t => new TransacaoDto { Id = t.Id, Descricao = t.Descricao }).ToList();

            _mockTransacaoRepositorio.Setup(r => r.ObterPorPessoaComIncludesAsync(pessoaId)).ReturnsAsync(transacoes);
            _mockMapper.Setup(m => m.Map<IEnumerable<TransacaoDto>>(transacoes)).Returns(transacaoDtos);

            // Act
            var resultado = await _transacaoService.ObterTransacoesPorPessoaAsync(pessoaId);

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task ObterTransacoesPorCategoriaAsync_DeveRetornarTransacoesDaCategoria()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var transacoes = new List<Transacao>
            {
                new Transacao { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Compra 1" },
                new Transacao { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Compra 2" }
            };

            var transacaoDtos = transacoes.Select(t => new TransacaoDto { Id = t.Id, Descricao = t.Descricao }).ToList();

            _mockTransacaoRepositorio.Setup(r => r.ObterPorCategoriaComIncludesAsync(categoriaId)).ReturnsAsync(transacoes);
            _mockMapper.Setup(m => m.Map<IEnumerable<TransacaoDto>>(transacoes)).Returns(transacaoDtos);

            // Act
            var resultado = await _transacaoService.ObterTransacoesPorCategoriaAsync(categoriaId);

            // Assert
            resultado.Should().HaveCount(2);
        }
    }
}