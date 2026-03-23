using Microsoft.AspNetCore.Mvc;
using ControleGastosResidenciais.Core.Interfaces;
using ControleGastosResidenciais.Core.DTOs;

namespace ControleGastosResidenciais.API.Controllers
{
    /// <summary>
    /// Endpoints da API para gerenciamento de categorias
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _servicoCategoria;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService servicoCategoria, ILogger<CategoriasController> logger)
        {
            _servicoCategoria = servicoCategoria;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as categorias
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> ObterCategorias()
        {
            try
            {
                var categorias = await _servicoCategoria.ObterTodasCategoriasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter categorias");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Criar uma nova categoria
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> CriarCategoria([FromBody] CriarCategoriaDto criarCategoriaDto)
        {
            try
            {
                var categoria = await _servicoCategoria.CriarCategoriaAsync(criarCategoriaDto);
                return CreatedAtAction(nameof(ObterCategoria), new { id = categoria.Id }, categoria);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obter uma categoria específica pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> ObterCategoria(Guid id)
        {
            try
            {
                var categoria = await _servicoCategoria.ObterCategoriaPorIdAsync(id);
                if (categoria == null)
                    return NotFound($"Categoria com ID {id} não encontrada");
                
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter categoria {Id}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }
    }
}