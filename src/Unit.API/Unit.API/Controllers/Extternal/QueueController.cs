using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;

namespace Unit.API.Controllers.Extternal
{
    [ApiController]
    [Area("Externo")]
    [Route("v1/[area]/queue")]
    
    public class QueueController : ControllerBase<QueueController>
    {
        private readonly ApiKeyAuthFilter _apiKeyAuthFilter;
        readonly ILogger<QueueController> _logger;
        readonly Unit.Application.Sevices.IQueueService _Service;


        public QueueController(ILogger<QueueController> logger, Unit.Application.Sevices.IQueueService Service)
        {
            _logger = logger;
            _Service = Service;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var dados = await _Service.GetOne(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost("proccess-queue")]
        //[ServiceFilter(typeof(ApiKeyAuthFilter))] // Aplica o filtro de autenticação por API Key
        public async Task<IActionResult> ProcessQueue([FromBody] QueryQueueForProcess parameter)
        {
            _logger.LogInformation($"Processando a fila");

            var rtn = await _Service.ProcessQueue(parameter);

            if(rtn.Success == false)
            {
                _logger.LogError($"Erro ao processar a fila: {string.Join(", ", rtn.Messages)}");
                return BadRequest(rtn);
            }

            return Ok(rtn);
        }

        [HttpPost("graphics")]
        //[ServiceFilter(typeof(ApiKeyAuthFilter))] // Aplica o filtro de autenticação por API Key
        public async Task<IActionResult> GraphicsQueue([FromBody] QueryQueueGraphicsRequest parameter)
        {
            _logger.LogInformation($"Gerando gráficos");

            var rtn = await _Service.GetGraphics(parameter);

            if (rtn.Success == false)
            {
                _logger.LogError($"Erro ao gerar gráficos.");
                return BadRequest(rtn);
            }

            return Ok(rtn);
        }
    }
}
