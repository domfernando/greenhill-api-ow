using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Unit.Application.DTOs.Request;
using Unit.Infra.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/evento")]
    [ApiController]
    public class EventoController : ControllerBase<EventoController>
    {
        readonly ILogger<EventoController> _logger;
        readonly Unit.Application.Sevices.IEventoService _Service;
        private readonly IHubContext<MySocketService> _hubContext;

        public EventoController(ILogger<EventoController> logger, Unit.Application.Sevices.IEventoService service, IHubContext<MySocketService> hubContext)
        {
            _logger = logger;
            _Service = service;
            _hubContext = hubContext;
        }

        #region CRUD

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryEventoRequest condicao)
        {
            var dados = await _Service.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            var usuario = User.Identity?.Name; // Ou use outro claim se necessário
            //if (!string.IsNullOrEmpty(usuario))
            //{
            //    try
            //    {
            //        //await _hubContext.Clients.User(usuario).SendAsync("Notificacao", usuario, "O processo foi finalizado!");
            //        //await _hubContext.Clients.All.SendAsync("Notificacao", usuario, "Para toddos");
            //        await _hubContext.Clients.User(usuario).SendAsync("Notificar", usuario, "O processo foi finalizado!");
            //        await _hubContext.Clients.All.SendAsync("Notificar", usuario, $"Eventos consultados");
            //    }
            //    catch (Exception ex)
            //    {

            //        throw;
            //    }

            //}
            return Ok(dados);
        }

        [HttpGet("papeis")]
        [Authorize]
        public async Task<IActionResult> GetByPapeis([FromQuery] QueryEventoByPapelRequest condicao)
        {
            var dados = await _Service.GetByPapel(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEventoRequest command)
        {
            var dados = await _Service.Add(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventoRequest command)
        {
            command.Id = id;
            var dados = await _Service.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion       
    }
}
