using ControleGastosResidenciais.Core.DTOs;

namespace ControleGastosResidenciais.Core.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> ObterTodasCategoriasAsync();
        Task<CategoriaDto> CriarCategoriaAsync(CriarCategoriaDto criarCategoriaDto);
    }
}