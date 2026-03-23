using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Enums;
using ControleGastosResidenciais.Core.Interfaces;

namespace ControleGastosResidenciais.Infraestrutura.Services
{
    /// <summary>
    /// Serviço para geração de relatórios de resumo
    /// </summary>
    public class RelatorioService : IRelatorioService
    {
        private readonly IPessoaRepositorio _pessoaRepositorio;
        private readonly ITransacaoRepositorio _transacaoRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;

        public RelatorioService(IPessoaRepositorio pessoaRepositorio, ITransacaoRepositorio transacaoRepositorio, ICategoriaRepositorio categoriaRepositorio)
        {
            _pessoaRepositorio = pessoaRepositorio;
            _transacaoRepositorio = transacaoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
        }

        /// <summary>
        /// Gera relatório de totais agrupados por pessoa
        /// </summary>
        public async Task<RelatorioTotaisPessoaDto> ObterRelatorioTotaisPorPessoaAsync()
        {
            var pessoas = await _pessoaRepositorio.ObterTodasComTransacoesAsync();
            
            var totaisPessoa = new List<TotaisPessoaDto>();
            decimal receitaTotalGeral = 0;
            decimal despesaTotalGeral = 0;
            
            foreach (var pessoa in pessoas)
            {
                var receita = pessoa.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor);
                
                var despesa = pessoa.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor);
                
                totaisPessoa.Add(new TotaisPessoaDto
                {
                    PessoaId = pessoa.Id,
                    NomePessoa = pessoa.Nome,
                    TotalReceita = receita,
                    TotalDespesa = despesa,
                    Saldo = receita - despesa
                });
                
                receitaTotalGeral += receita;
                despesaTotalGeral += despesa;
            }
            
            // Ordenar por nome
            totaisPessoa = totaisPessoa.OrderBy(p => p.NomePessoa).ToList();
            
            return new RelatorioTotaisPessoaDto
            {
                TotaisPessoa = totaisPessoa,
                Resumo = new TotaisResumoDto
                {
                    ReceitaTotal = receitaTotalGeral,
                    DespesaTotal = despesaTotalGeral,
                    SaldoLiquido = receitaTotalGeral - despesaTotalGeral
                }
            };
        }

        /// <summary>
        /// Gera relatório de totais agrupados por categoria (Opcional)
        /// </summary>
        public async Task<RelatorioTotaisCategoriaDto> ObterRelatorioTotaisPorCategoriaAsync()
        {
            var categorias = await _categoriaRepositorio.ObterTodasComTransacoesAsync();
            
            var totaisCategoria = new List<TotaisCategoriaDto>();
            decimal receitaTotalGeral = 0;
            decimal despesaTotalGeral = 0;
            
            foreach (var categoria in categorias)
            {
                var receita = categoria.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor);
                
                var despesa = categoria.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor);
                
                totaisCategoria.Add(new TotaisCategoriaDto
                {
                    CategoriaId = categoria.Id,
                    DescricaoCategoria = categoria.Descricao,
                    FinalidadeCategoria = categoria.Finalidade.ToString(),
                    TotalReceita = receita,
                    TotalDespesa = despesa,
                    Saldo = receita - despesa
                });
                
                receitaTotalGeral += receita;
                despesaTotalGeral += despesa;
            }
            
            // Ordenar por descrição
            totaisCategoria = totaisCategoria.OrderBy(c => c.DescricaoCategoria).ToList();
            
            return new RelatorioTotaisCategoriaDto
            {
                TotaisCategoria = totaisCategoria,
                Resumo = new TotaisResumoDto
                {
                    ReceitaTotal = receitaTotalGeral,
                    DespesaTotal = despesaTotalGeral,
                    SaldoLiquido = receitaTotalGeral - despesaTotalGeral
                }
            };
        }
    }
}