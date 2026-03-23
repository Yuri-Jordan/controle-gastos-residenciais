using ControleGastosResidenciais.Core.DTOs;

namespace ControleGastosResidenciais.Core.Interfaces
{
    public interface IRelatorioService
    {
        Task<RelatorioTotaisPessoaDto> ObterRelatorioTotaisPorPessoaAsync();
        Task<RelatorioTotaisCategoriaDto> ObterRelatorioTotaisPorCategoriaAsync();
    }
}