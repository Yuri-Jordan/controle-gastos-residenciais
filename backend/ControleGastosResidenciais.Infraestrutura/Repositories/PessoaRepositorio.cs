using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosResidenciais.Infraestrutura.Repositories
{
    /// <summary>
    /// Implementação do repositório de pessoas
    /// </summary>
    public class PessoaRepositorio : Repositorio<Pessoa>, IPessoaRepositorio
    {
        public PessoaRepositorio(ApplicationDbContext contexto) : base(contexto)
        {
        }

        public async Task<IEnumerable<Pessoa>> ObterTodasComTransacoesAsync()
        {
            return await _contexto.Pessoas
                .Include(p => p.Transacoes)
                .ToListAsync();
        }
    }
}