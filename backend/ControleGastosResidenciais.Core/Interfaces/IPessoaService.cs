using ControleGastosResidenciais.Core.DTOs;

namespace ControleGastosResidenciais.Core.Interfaces
{
    public interface IPessoaService
    {
        Task<IEnumerable<PessoaDto>> ObterTodasPessoasAsync();
        Task<PessoaDto?> ObterPessoaPorIdAsync(Guid id);
        Task<PessoaDto> CriarPessoaAsync(CriarPessoaDto criarPessoaDto);
        Task<PessoaDto> AtualizarPessoaAsync(Guid id, AtualizarPessoaDto atualizarPessoaDto);
        Task ExcluirPessoaAsync(Guid id);
    }
}