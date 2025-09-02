using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Infra.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/papel")]
    [ApiController]
    public class PapelController : ControllerBase<PapelController>
    {
        readonly ILogger<PapelController> _logger;
        readonly Unit.Application.Sevices.IPapelService _Service;
        readonly IPubService _pubService;

        public PapelController(ILogger<PapelController> logger, Unit.Application.Sevices.IPapelService service, IPubService pubService)
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
        public async Task<IActionResult> GetAll([FromQuery] QueryPapelRequest condicao)
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
        public async Task<IActionResult> Create([FromBody] CreatePapelRequest command)
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePapelRequest command)
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
        [Route("removerPubPapel")]
        [Authorize]
        public async Task<IActionResult> RemovePerfil([FromBody] RemovePubPapelRequest command)
        {
            var dados = await _pubService.RemPapel(command.Id);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("adicionarPubPapel")]
        [Authorize]
        public async Task<IActionResult> AdicionaPerfil([FromBody] AdicionaPubPapelRequest command)
        {
            var dados = await _pubService.AddPapel(command.PubId, command.PapelId);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion       
    }
}
