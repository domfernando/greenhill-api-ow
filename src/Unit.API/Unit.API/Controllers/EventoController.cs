using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;

namespace Unit.API.Controllers
{
    [Route("/v1/evento")]
    [ApiController]
    public class EventoController : ControllerBase<EventoController>
    {
        readonly ILogger<EventoController> _logger;
        readonly Unit.Application.Sevices.IEventoService _Service;

        public EventoController(ILogger<EventoController> logger, Unit.Application.Sevices.IEventoService service)
        {
            _logger = logger;
            _Service = service;
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
