using Microsoft.AspNetCore.Mvc;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Interfaces;

namespace ControleGastosResidenciais.API.Controllers
{
    /// <summary>
    /// Endpoints da API para gerenciamento de transações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _servicoTransacao;
        private readonly ILogger<TransacoesController> _logger;

        public TransacoesController(ITransacaoService servicoTransacao, ILogger<TransacoesController> logger)
        {
            _servicoTransacao = servicoTransacao;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as transações
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoDto>>> ObterTransacoes()
        {
            try
            {
                var transacoes = await _servicoTransacao.ObterTodasTransacoesAsync();
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter transações");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obter uma transação específica pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransacaoDto>> ObterTransacao(Guid id)
        {
            try
            {
                var transacao = await _servicoTransacao.ObterTransacaoPorIdAsync(id);
                if (transacao == null)
                    return NotFound($"Transação com ID {id} não encontrada");
                
                return Ok(transacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter transação {Id}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Criar uma nova transação com validação de regras de negócio
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransacaoDto>> CriarTransacao([FromBody] CriarTransacaoDto criarTransacaoDto)
        {
            try
            {
                var transacao = await _servicoTransacao.CriarTransacaoAsync(criarTransacaoDto);
                return CreatedAtAction(nameof(ObterTransacao), new { id = transacao.Id }, transacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar transação");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obter transações por pessoa
        /// </summary>
        [HttpGet("pessoa/{pessoaId}")]
        public async Task<ActionResult<IEnumerable<TransacaoDto>>> ObterTransacoesPorPessoa(Guid pessoaId)
        {
            try
            {
                var transacoes = await _servicoTransacao.ObterTransacoesPorPessoaAsync(pessoaId);
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter transações da pessoa {PessoaId}", pessoaId);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obter transações por categoria
        /// </summary>
        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<TransacaoDto>>> ObterTransacoesPorCategoria(Guid categoriaId)
        {
            try
            {
                var transacoes = await _servicoTransacao.ObterTransacoesPorCategoriaAsync(categoriaId);
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter transações da categoria {CategoriaId}", categoriaId);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }
    }
}