using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Enums;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Services;
using FluentAssertions;
using Moq;

namespace ControleGastosResidenciais.Testes.UnitTests.Services
{
    public class RelatorioServiceTests
    {
        private readonly Mock<IPessoaRepositorio> _mockPessoaRepositorio;
        private readonly Mock<ITransacaoRepositorio> _mockTransacaoRepositorio;
        private readonly Mock<ICategoriaRepositorio> _mockCategoriaRepositorio;
        private readonly RelatorioService _relatorioService;

        public RelatorioServiceTests()
        {
            _mockPessoaRepositorio = new Mock<IPessoaRepositorio>();
            _mockTransacaoRepositorio = new Mock<ITransacaoRepositorio>();
            _mockCategoriaRepositorio = new Mock<ICategoriaRepositorio>();
            _relatorioService = new RelatorioService(_mockPessoaRepositorio.Object, _mockTransacaoRepositorio.Object, _mockCategoriaRepositorio.Object);
        }

        [Fact]
        public async Task ObterRelatorioTotaisPorPessoaAsync_DeveRetornarRelatorioComTotaisCorretos()
        {
            // Arrange
            var pessoa1 = new Pessoa { Id = Guid.NewGuid(), Nome = "João Silva" };
            var pessoa2 = new Pessoa { Id = Guid.NewGuid(), Nome = "Maria Santos" };

            var transacoes = new List<Transacao>
            {
                new Transacao { Id = Guid.NewGuid(), Pessoa = pessoa1, Valor = 1000, Tipo = TipoTransacao.Receita },
                new Transacao { Id = Guid.NewGuid(), Pessoa = pessoa1, Valor = 500, Tipo = TipoTransacao.Despesa },
                new Transacao { Id = Guid.NewGuid(), Pessoa = pessoa2, Valor = 800, Tipo = TipoTransacao.Receita },
                new Transacao { Id = Guid.NewGuid(), Pessoa = pessoa2, Valor = 300, Tipo = TipoTransacao.Despesa }
            };

            pessoa1.Transacoes = transacoes.Where(t => t.Pessoa == pessoa1).ToList();
            pessoa2.Transacoes = transacoes.Where(t => t.Pessoa == pessoa2).ToList();

            var pessoas = new List<Pessoa> { pessoa1, pessoa2 };

            _mockPessoaRepositorio.Setup(r => r.ObterTodasComTransacoesAsync()).ReturnsAsync(pessoas);

            // Act
            var resultado = await _relatorioService.ObterRelatorioTotaisPorPessoaAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotaisPessoa.Should().HaveCount(2);

            var joao = resultado.TotaisPessoa.First(p => p.NomePessoa == "João Silva");
            joao.TotalReceita.Should().Be(1000);
            joao.TotalDespesa.Should().Be(500);
            joao.Saldo.Should().Be(500);

            var maria = resultado.TotaisPessoa.First(p => p.NomePessoa == "Maria Santos");
            maria.TotalReceita.Should().Be(800);
            maria.TotalDespesa.Should().Be(300);
            maria.Saldo.Should().Be(500);

            resultado.Resumo.ReceitaTotal.Should().Be(1800);
            resultado.Resumo.DespesaTotal.Should().Be(800);
            resultado.Resumo.SaldoLiquido.Should().Be(1000);
        }

        [Fact]
        public async Task ObterRelatorioTotaisPorCategoriaAsync_DeveRetornarRelatorioComTotaisCorretos()
        {
            // Arrange
            var categoria1 = new Categoria { Id = Guid.NewGuid(), Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa };
            var categoria2 = new Categoria { Id = Guid.NewGuid(), Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita };

            var transacoes = new List<Transacao>
            {
                new Transacao { Id = Guid.NewGuid(), Categoria = categoria1, Valor = 500, Tipo = TipoTransacao.Despesa },
                new Transacao { Id = Guid.NewGuid(), Categoria = categoria1, Valor = 300, Tipo = TipoTransacao.Despesa },
                new Transacao { Id = Guid.NewGuid(), Categoria = categoria2, Valor = 2000, Tipo = TipoTransacao.Receita },
                new Transacao { Id = Guid.NewGuid(), Categoria = categoria2, Valor = 500, Tipo = TipoTransacao.Receita }
            };

            categoria1.Transacoes = transacoes.Where(t => t.Categoria == categoria1).ToList();
            categoria2.Transacoes = transacoes.Where(t => t.Categoria == categoria2).ToList();

            var categorias = new List<Categoria> { categoria1, categoria2 };

            _mockCategoriaRepositorio.Setup(r => r.ObterTodasComTransacoesAsync()).ReturnsAsync(categorias);

            // Act
            var resultado = await _relatorioService.ObterRelatorioTotaisPorCategoriaAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotaisCategoria.Should().HaveCount(2);

            var alimentacao = resultado.TotaisCategoria.First(c => c.DescricaoCategoria == "Alimentação");
            alimentacao.TotalReceita.Should().Be(0);
            alimentacao.TotalDespesa.Should().Be(800);
            alimentacao.Saldo.Should().Be(-800);
            alimentacao.FinalidadeCategoria.Should().Be("Despesa");

            var salario = resultado.TotaisCategoria.First(c => c.DescricaoCategoria == "Salário");
            salario.TotalReceita.Should().Be(2500);
            salario.TotalDespesa.Should().Be(0);
            salario.Saldo.Should().Be(2500);
            salario.FinalidadeCategoria.Should().Be("Receita");

            resultado.Resumo.ReceitaTotal.Should().Be(2500);
            resultado.Resumo.DespesaTotal.Should().Be(800);
            resultado.Resumo.SaldoLiquido.Should().Be(1700);
        }

        [Fact]
        public async Task ObterRelatorioTotaisPorPessoaAsync_ComPessoasSemTransacoes_DeveRetornarTotaisZerados()
        {
            // Arrange
            var pessoa = new Pessoa { Id = Guid.NewGuid(), Nome = "João Silva", Transacoes = new List<Transacao>() };
            var pessoas = new List<Pessoa> { pessoa };

            _mockPessoaRepositorio.Setup(r => r.ObterTodasComTransacoesAsync()).ReturnsAsync(pessoas);

            // Act
            var resultado = await _relatorioService.ObterRelatorioTotaisPorPessoaAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotaisPessoa.Should().HaveCount(1);

            var joao = resultado.TotaisPessoa.First();
            joao.TotalReceita.Should().Be(0);
            joao.TotalDespesa.Should().Be(0);
            joao.Saldo.Should().Be(0);

            resultado.Resumo.ReceitaTotal.Should().Be(0);
            resultado.Resumo.DespesaTotal.Should().Be(0);
            resultado.Resumo.SaldoLiquido.Should().Be(0);
        }
    }
}