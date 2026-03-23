using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastosResidenciais.API.Controllers
{
    /// <summary>
    /// Endpoints da API para gerenciamento de pessoas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _servicoPessoa;
        private readonly ILogger<PessoasController> _logger;

        public PessoasController(IPessoaService servicoPessoa, ILogger<PessoasController> logger)
        {
            _servicoPessoa = servicoPessoa;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as pessoas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDto>>> ObterPessoas()
        {
            try
            {
                var pessoas = await _servicoPessoa.ObterTodasPessoasAsync();
                return Ok(pessoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pessoas");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obter uma pessoa específica pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDto>> ObterPessoa(Guid id)
        {
            try
            {
                var pessoa = await _servicoPessoa.ObterPessoaPorIdAsync(id);
                if (pessoa == null)
                    return NotFound($"Pessoa com ID {id} não encontrada");
                
                return Ok(pessoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pessoa {Id}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Criar uma nova pessoa
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PessoaDto>> CriarPessoa([FromBody] CriarPessoaDto criarPessoaDto)
        {
            try
            {
                var pessoa = await _servicoPessoa.CriarPessoaAsync(criarPessoaDto);
                return CreatedAtAction(nameof(ObterPessoa), new { id = pessoa.Id }, pessoa);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pessoa");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Atualizar uma pessoa existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PessoaDto>> AtualizarPessoa(Guid id, [FromBody] AtualizarPessoaDto atualizarPessoaDto)
        {
            try
            {
                var pessoa = await _servicoPessoa.AtualizarPessoaAsync(id, atualizarPessoaDto);
                return Ok(pessoa);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pessoa {Id}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Excluir uma pessoa (exclusão em cascata remove todas as transações associadas)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirPessoa(Guid id)
        {
            try
            {
                await _servicoPessoa.ExcluirPessoaAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pessoa {Id}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }
    }
}