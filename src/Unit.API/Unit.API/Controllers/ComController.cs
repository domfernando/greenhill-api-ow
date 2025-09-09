using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Domain.Entities.Config;
using Unit.Infra.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/com")]
    [ApiController]
    public class CommController : ControllerBase<CommController>
    {
        readonly ILogger<CommController> _logger;
        //private readonly IHubContext<MySocketService> _hubContext;
        readonly IMySocketService _mySocketService;

        public CommController(ILogger<CommController> logger, IMySocketService mySocketService)
        {
            _logger = logger;
            //_hubContext = hubContext;
            _mySocketService = mySocketService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ComNotificarRequest comando)
        {
            try
            {
                await _mySocketService.SendNotificacao(comando);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("notificar")]
        public async Task<IActionResult> Notificar([FromBody] ComNotificarRequest condicao)
        {
            if (!string.IsNullOrEmpty(condicao.Usuario))
            {
                try
                {
                    await _mySocketService.SendNotificacao(condicao);
                    _logger.LogInformation($"Notificação enviada para o usuário {condicao.Usuario}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao enviar notificação para o usuário {condicao.Usuario}");
                }

            }
            return Ok();
        }

    }
}
