using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Infraestrutura.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastosResidenciais.API.Controllers
{
    /// <summary>
    /// Endpoints da API para geração de relatórios de resumo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatorioService _servicoRelatorio;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(RelatorioService servicoRelatorio, ILogger<RelatoriosController> logger)
        {
            _servicoRelatorio = servicoRelatorio;
            _logger = logger;
        }

        /// <summary>
        /// Obter relatório de totais agrupados por pessoa
        /// </summary>
        [HttpGet("pessoas")]
        public async Task<ActionResult<RelatorioTotaisPessoaDto>> ObterRelatorioTotaisPorPessoa()
        {
            try
            {
                var relatorio = await _servicoRelatorio.ObterRelatorioTotaisPorPessoaAsync();
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de totais por pessoa");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obter relatório de totais agrupados por categoria (Opcional)
        /// </summary>
        [HttpGet("categorias")]
        public async Task<ActionResult<RelatorioTotaisCategoriaDto>> ObterRelatorioTotaisPorCategoria()
        {
            try
            {
                var relatorio = await _servicoRelatorio.ObterRelatorioTotaisPorCategoriaAsync();
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de totais por categoria");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }
    }
}