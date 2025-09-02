using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/grupo")]
    [ApiController]
    public class GrupoController : ControllerBase<GrupoController>
    {
        readonly ILogger<GrupoController> _logger;
        readonly Unit.Application.Sevices.IGrupoService _Service;
        readonly IPubService _pubService;

        public GrupoController(ILogger<GrupoController> logger, Unit.Application.Sevices.IGrupoService service, IPubService pubService)
        {
            _logger = logger;
            _Service = service;
            _pubService = pubService;
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
        public async Task<IActionResult> GetAll([FromQuery] QueryGrupoRequest condicao)
        {
            var dados = await _Service.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateGrupoRequest command)
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGrupoRequest command)
        {
            command.Id = id;
            var dados = await _Service.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("removerPubGrupo")]
        [Authorize]
        public async Task<IActionResult> RemovePublicador([FromBody] RemoveGrupoPubRequest command)
        {
            var dados = await _pubService.RemGrupo(command.Id);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("adicionarPubGrupo")]
        [Authorize]
        public async Task<IActionResult> AdicionaPerfil([FromBody] AdicionaGrupoPubRequest command)
        {
            var dados = await _pubService.AddGrupo(command.PubId, command.GrupoId, command.Papel);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet("publicador/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPublicador([FromRoute] int id)
        {
            var dados = await _Service.GetGrupoPub(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("publicador/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePublicador(int id, [FromBody] UpdateGrupoPubRequest command)
        {
            command.Id = id;
            var dados = await _Service.UpdateGrupoPub(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion       
    }
}
