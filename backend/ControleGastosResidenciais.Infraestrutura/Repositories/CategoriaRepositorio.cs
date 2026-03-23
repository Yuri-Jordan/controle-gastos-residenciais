using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Data;
using ControleGastosResidenciais.Infraestrutura.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosResidenciais.Infraestrutura.Repositories
{
    /// <summary>
    /// Implementação do repositório de categorias
    /// </summary>
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        public CategoriaRepositorio(ApplicationDbContext contexto) : base(contexto)
        {
        }

        public async Task<IEnumerable<Categoria>> ObterTodasComTransacoesAsync()
        {
            return await _contexto.Categorias
                .Include(c => c.Transacoes)
                .ToListAsync();
        }
    }
}