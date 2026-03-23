using ControleGastosResidenciais.Core.Entities;

namespace ControleGastosResidenciais.Core.Interfaces
{
    /// <summary>
    /// Interface específica para repositório de pessoas
    /// </summary>
    public interface IPessoaRepositorio : IRepositorio<Pessoa>
    {
        Task<IEnumerable<Pessoa>> ObterTodasComTransacoesAsync();
    }
}