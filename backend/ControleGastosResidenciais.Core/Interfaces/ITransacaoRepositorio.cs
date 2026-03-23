using ControleGastosResidenciais.Core.Entities;

namespace ControleGastosResidenciais.Core.Interfaces
{
    /// <summary>
    /// Interface específica para repositório de transações
    /// </summary>
    public interface ITransacaoRepositorio : IRepositorio<Transacao>
    {
        Task<IEnumerable<Transacao>> ObterTodasComIncludesAsync();
        Task<Transacao?> ObterPorIdComIncludesAsync(Guid id);
        Task<IEnumerable<Transacao>> ObterPorPessoaComIncludesAsync(Guid pessoaId);
        Task<IEnumerable<Transacao>> ObterPorCategoriaComIncludesAsync(Guid categoriaId);
    }
}