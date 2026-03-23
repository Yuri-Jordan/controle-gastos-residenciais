using ControleGastosResidenciais.Core.Enums;

namespace ControleGastosResidenciais.Core.DTOs
{
    public class CategoriaDto
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public FinalidadeCategoria Finalidade { get; set; }
        public string ExibirFinalidade { get; set; } = string.Empty;
    }

    public class CriarCategoriaDto
    {
        public string Descricao { get; set; } = string.Empty;
        public FinalidadeCategoria Finalidade { get; set; }
    }
}