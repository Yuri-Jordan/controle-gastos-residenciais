using ControleGastosResidenciais.Core.Enums;

namespace ControleGastosResidenciais.Core.DTOs
{
    public class TransacaoDto
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public string ExibirTipo { get; set; } = string.Empty;
        public Guid CategoriaId { get; set; }
        public string DescricaoCategoria { get; set; } = string.Empty;
        public Guid PessoaId { get; set; }
        public string NomePessoa { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
    }

    public class CriarTransacaoDto
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public Guid CategoriaId { get; set; }
        public Guid PessoaId { get; set; }
    }
}