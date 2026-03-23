using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Infrastructure.Data;
using ControleGastosResidenciais.Core.Interfaces;

namespace ControleGastosResidenciais.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação genérica de repositório utilizando Entity Framework Core
    /// </summary>
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        protected readonly ApplicationDbContext _contexto;
        protected readonly DbSet<T> _dbSet;

        public Repositorio(ApplicationDbContext contexto)
        {
            _contexto = contexto;
            _dbSet = contexto.Set<T>();
        }

        public async Task<T?> ObterPorIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.Where(predicado).ToListAsync();
        }

        public async Task<T> AdicionarAsync(T entidade)
        {
            await _dbSet.AddAsync(entidade);
            await _contexto.SaveChangesAsync();
            return entidade;
        }

        public async Task AtualizarAsync(T entidade)
        {
            _dbSet.Update(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync(T entidade)
        {
            _dbSet.Remove(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.AnyAsync(predicado);
        }
    }
}