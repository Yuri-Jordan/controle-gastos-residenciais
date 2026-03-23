using ControleGastosResidenciais.Core.Entities;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosResidenciais.Infraestrutura.Repositories
{
    /// <summary>
    /// Implementação do repositório de transações
    /// </summary>
    public class TransacaoRepositorio : Repositorio<Transacao>, ITransacaoRepositorio
    {
        public TransacaoRepositorio(ApplicationDbContext contexto) : base(contexto)
        {
        }

        public async Task<Transacao?> ObterPorIdComIncludesAsync(Guid id)
        {
            return await _contexto.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transacao>> ObterPorPessoaComIncludesAsync(Guid pessoaId)
        {
            return await _contexto.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .Where(t => t.PessoaId == pessoaId)
                .OrderByDescending(t => t.CriadoEm)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterPorCategoriaComIncludesAsync(Guid categoriaId)
        {
            return await _contexto.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .Where(t => t.CategoriaId == categoriaId)
                .OrderByDescending(t => t.CriadoEm)
                .ToListAsync();
        }

        public Task<IEnumerable<Transacao>> ObterTodasComIncludesAsync()
        {
            throw new NotImplementedException();
        }
    }
}