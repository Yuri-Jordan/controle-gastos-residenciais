using ControleGastosResidenciais.Core.DTOs;

namespace ControleGastosResidenciais.Core.Interfaces
{
    public interface ITransacaoService
    {
        Task<IEnumerable<TransacaoDto>> ObterTodasTransacoesAsync();
        Task<TransacaoDto?> ObterTransacaoPorIdAsync(Guid id);
        Task<TransacaoDto> CriarTransacaoAsync(CriarTransacaoDto criarTransacaoDto);
        Task ExcluirTransacaoAsync(Guid id);
        Task<IEnumerable<TransacaoDto>> ObterTransacoesPorPessoaAsync(Guid pessoaId);
        Task<IEnumerable<TransacaoDto>> ObterTransacoesPorCategoriaAsync(Guid categoriaId);
    }
}