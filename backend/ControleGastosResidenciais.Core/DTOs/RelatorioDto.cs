namespace ControleGastosResidenciais.Core.DTOs
{
    public class RelatorioTotaisPessoaDto
    {
        public List<TotaisPessoaDto> TotaisPessoa { get; set; } = new();
        public TotaisResumoDto Resumo { get; set; } = new();
    }

    public class TotaisPessoaDto
    {
        public Guid PessoaId { get; set; }
        public string NomePessoa { get; set; } = string.Empty;
        public decimal TotalReceita { get; set; }
        public decimal TotalDespesa { get; set; }
        public decimal Saldo { get; set; }
    }

    public class RelatorioTotaisCategoriaDto
    {
        public List<TotaisCategoriaDto> TotaisCategoria { get; set; } = new();
        public TotaisResumoDto Resumo { get; set; } = new();
    }

    public class TotaisCategoriaDto
    {
        public Guid CategoriaId { get; set; }
        public string DescricaoCategoria { get; set; } = string.Empty;
        public string FinalidadeCategoria { get; set; } = string.Empty;
        public decimal TotalReceita { get; set; }
        public decimal TotalDespesa { get; set; }
        public decimal Saldo { get; set; }
    }

    public class TotaisResumoDto
    {
        public decimal ReceitaTotal { get; set; }
        public decimal DespesaTotal { get; set; }
        public decimal SaldoLiquido { get; set; }
    }
}