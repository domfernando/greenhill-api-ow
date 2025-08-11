using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Application.Util;

namespace Unit.API.Controllers
{
    [Route("/v1/mensagem")]
    [ApiController]
    public class MensagemController : ControllerBase<MensagemController>
    {
        readonly ILogger<MensagemController> _logger;
        readonly Unit.Application.Sevices.IMensagemService _Service;

        public MensagemController(ILogger<MensagemController> logger, Unit.Application.Sevices.IMensagemService service)
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
        public async Task<IActionResult> GetAll([FromQuery] QueryMensagemRequest condicao)
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
        public async Task<IActionResult> Create([FromBody] CreateMensagemRequest command)
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMensagemRequest command)
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
