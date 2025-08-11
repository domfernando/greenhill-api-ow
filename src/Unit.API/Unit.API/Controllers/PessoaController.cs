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
    [Route("/v1/pessoa")]
    [ApiController]
    public class PessoaController : ControllerBase<PessoaController>
    {
        readonly ILogger<PessoaController> _logger;
        readonly Unit.Application.Sevices.IPessoaService _Service;
        readonly Unit.Application.Sevices.IEnderecoService _svcEndereco;

        public PessoaController(ILogger<PessoaController> logger, Unit.Application.Sevices.IPessoaService service, IEnderecoService svcEndereco)
        {
            _logger = logger;
            _Service = service;
            _svcEndereco = svcEndereco;
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
        public async Task<IActionResult> GetAll([FromQuery] QueryPessoaRequest condicao)
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
        public async Task<IActionResult> Create([FromBody] CreatePessoaRequest command)
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePessoaRequest command)
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

        #region Papel

        [HttpPost("add-papel")]
        [Authorize]
        public async Task<IActionResult> AddPapel([FromBody] AddPapelPessoaRequest command)
        {
            var dados = await _Service.AddPapel(command.PessoaId, command.PapelId);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpDelete("rem-papel")]
        [Authorize]
        public async Task<IActionResult> RemPapel(int id)
        {
            var dados = await _Service.RemPapel(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion

        #region Endereço

        [HttpPost("add-endereco")]
        [Authorize]
        public async Task<IActionResult> AddEndereco([FromBody] CreateEnderecoRequest command)
        {
            var dados = await _svcEndereco.Add(command, command.PessoaId);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("update-endereco/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEndereco(int id, [FromBody] UpdateEnderecoRequest command)
        {
            command.Id = id;
            var dados = await _svcEndereco.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion

        [HttpPost("graphics")]
        [Authorize]
        public async Task<IActionResult> Graphics()
        {
            _logger.LogInformation($"Gerando gráficos");

            var rtn = await _Service.GetGraphics();

            if (rtn.Success == false)
            {
                _logger.LogError($"Erro ao gerar gráficos.");
                return BadRequest(rtn);
            }

            return Ok(rtn);
        }
    }
}
