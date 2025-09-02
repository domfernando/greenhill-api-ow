using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/pub")]
    [ApiController]
    public class PubController : ControllerBase<PubController>
    {
        readonly ILogger<PubController> _logger;
        readonly IPubService _Service;

        public PubController(ILogger<PubController> logger, IPubService service)
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
        public async Task<IActionResult> GetAll([FromQuery] PubQueryModel condicao)
        {
            var dados = await _Service.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet("lista")]
        public async Task<IActionResult> GetList([FromQuery] PubQueryModel condicao)
        {
            var dados = await _Service.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            if (dados.Data != null)
            {
                var lista = (from x in (List<PubResponse>)dados.Data
                             select new
                             {
                                 Value =x.ID,
                                 Text=x.Nome
                             }).ToList();
                dados.Data = lista;
            }

            return Ok(dados);
        }


        [HttpGet("usuario/{id}")]
        [Authorize]
        public async Task<IActionResult> GetByUser([FromRoute] int id)
        {
            var dados = await _Service.GetByUsuarioId(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] PubNewModel command)
        {
            var dados = await _Service.AddAsync(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] PubUpdateModel command)
        {
            command.ID = id;
            var dados = await _Service.UpdateAsync(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion       
    }
}
