using AutoMapper;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ControleGastosResidenciais.Testes.UnitTests.Services
{
    public class PessoaServiceTests
    {
        private readonly Mock<IRepositorio<Pessoa>> _mockRepositorio;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PessoaService _pessoaService;

        public PessoaServiceTests()
        {
            _mockRepositorio = new Mock<IRepositorio<Pessoa>>();
            _mockMapper = new Mock<IMapper>();
            _pessoaService = new PessoaService(_mockRepositorio.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task ObterTodasPessoasAsync_DeveRetornarListaDePessoasComEhMenorDeIdade()
        {
            // Arrange
            var pessoas = new List<Pessoa>
            {
                new Pessoa { Id = Guid.NewGuid(), Nome = "João Silva", Idade = 25 },
                new Pessoa { Id = Guid.NewGuid(), Nome = "Maria Santos", Idade = 16 }
            };

            var pessoaDtos = new List<PessoaDto>
            {
                new PessoaDto { Id = pessoas[0].Id, Nome = "João Silva", Idade = 25 },
                new PessoaDto { Id = pessoas[1].Id, Nome = "Maria Santos", Idade = 16 }
            };

            _mockRepositorio.Setup(r => r.ObterTodosAsync()).ReturnsAsync(pessoas);
            _mockMapper.Setup(m => m.Map<IEnumerable<PessoaDto>>(pessoas)).Returns(pessoaDtos);

            // Act
            var resultado = await _pessoaService.ObterTodasPessoasAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.First().EhMenorDeIdade.Should().BeFalse();
            resultado.Last().EhMenorDeIdade.Should().BeTrue();
        }

        [Fact]
        public async Task ObterPessoaPorIdAsync_ComIdExistente_DeveRetornarPessoaDto()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 25 };
            var pessoaDto = new PessoaDto { Id = pessoaId, Nome = "João Silva", Idade = 25 };

            _mockRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockMapper.Setup(m => m.Map<PessoaDto>(pessoa)).Returns(pessoaDto);

            // Act
            var resultado = await _pessoaService.ObterPessoaPorIdAsync(pessoaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be("João Silva");
            resultado.EhMenorDeIdade.Should().BeFalse();
        }

        [Fact]
        public async Task ObterPessoaPorIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            _mockRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync((Pessoa?)null);

            // Act
            var resultado = await _pessoaService.ObterPessoaPorIdAsync(pessoaId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CriarPessoaAsync_ComDadosValidos_DeveRetornarPessoaDto()
        {
            // Arrange
            var criarPessoaDto = new CriarPessoaDto
            {
                Nome = "João Silva",
                Idade = 25
            };

            var pessoa = new Pessoa
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Idade = 25
            };

            var pessoaDto = new PessoaDto
            {
                Id = pessoa.Id,
                Nome = "João Silva",
                Idade = 25
            };

            _mockMapper.Setup(m => m.Map<Pessoa>(criarPessoaDto)).Returns(pessoa);
            _mockRepositorio.Setup(r => r.AdicionarAsync(pessoa)).ReturnsAsync(pessoa);
            _mockMapper.Setup(m => m.Map<PessoaDto>(pessoa)).Returns(pessoaDto);

            // Act
            var resultado = await _pessoaService.CriarPessoaAsync(criarPessoaDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("João Silva");
            resultado.EhMenorDeIdade.Should().BeFalse();
        }

        [Fact]
        public async Task CriarPessoaAsync_ComNomeVazio_DeveLancarArgumentException()
        {
            // Arrange
            var criarPessoaDto = new CriarPessoaDto
            {
                Nome = "",
                Idade = 25
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _pessoaService.CriarPessoaAsync(criarPessoaDto));
        }

        [Fact]
        public async Task CriarPessoaAsync_ComIdadeNegativa_DeveLancarArgumentException()
        {
            // Arrange
            var criarPessoaDto = new CriarPessoaDto
            {
                Nome = "João Silva",
                Idade = -1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _pessoaService.CriarPessoaAsync(criarPessoaDto));
        }

        [Fact]
        public async Task AtualizarPessoaAsync_ComDadosValidos_DeveRetornarPessoaDto()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 25 };
            var atualizarPessoaDto = new AtualizarPessoaDto { Nome = "João Silva Atualizado", Idade = 26 };
            var pessoaDto = new PessoaDto { Id = pessoaId, Nome = "João Silva Atualizado", Idade = 26 };

            _mockRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockMapper.Setup(m => m.Map(atualizarPessoaDto, pessoa));
            _mockRepositorio.Setup(r => r.AtualizarAsync(pessoa)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<PessoaDto>(pessoa)).Returns(pessoaDto);

            // Act
            var resultado = await _pessoaService.AtualizarPessoaAsync(pessoaId, atualizarPessoaDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("João Silva Atualizado");
        }

        [Fact]
        public async Task AtualizarPessoaAsync_ComIdInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var atualizarPessoaDto = new AtualizarPessoaDto { Nome = "João Silva", Idade = 25 };

            _mockRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync((Pessoa?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _pessoaService.AtualizarPessoaAsync(pessoaId, atualizarPessoaDto));
        }

        [Fact]
        public async Task ExcluirPessoaAsync_ComIdExistente_DeveExecutarComSucesso()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var pessoa = new Pessoa { Id = pessoaId, Nome = "João Silva", Idade = 25 };

            _mockRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockRepositorio.Setup(r => r.ExcluirAsync(pessoa)).Returns(Task.CompletedTask);

            // Act
            await _pessoaService.ExcluirPessoaAsync(pessoaId);

            // Assert
            _mockRepositorio.Verify(r => r.ExcluirAsync(pessoa), Times.Once);
        }

        [Fact]
        public async Task ExcluirPessoaAsync_ComIdInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();

            _mockRepositorio.Setup(r => r.ObterPorIdAsync(pessoaId)).ReturnsAsync((Pessoa?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _pessoaService.ExcluirPessoaAsync(pessoaId));
        }
    }
}