using System.Linq.Expressions;

namespace ControleGastosResidenciais.Core.Interfaces
{
    /// <summary>
    /// Interface genérica de repositório para operações de acesso a dados
    /// </summary>
    public interface IRepositorio<T> where T : class
    {
        Task<T?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado);
        Task<T> AdicionarAsync(T entidade);
        Task AtualizarAsync(T entidade);
        Task ExcluirAsync(T entidade);
        Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado);
    }
}