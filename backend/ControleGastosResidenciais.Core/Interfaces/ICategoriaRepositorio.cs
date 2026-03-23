using ControleGastosResidenciais.Core.Entities;

namespace ControleGastosResidenciais.Core.Interfaces
{
    /// <summary>
    /// Interface específica para repositório de categorias
    /// </summary>
    public interface ICategoriaRepositorio : IRepositorio<Categoria>
    {
        Task<IEnumerable<Categoria>> ObterTodasComTransacoesAsync();
    }
}