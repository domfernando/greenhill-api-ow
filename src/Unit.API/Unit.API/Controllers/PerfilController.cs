using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Application.Util;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Unit.API.Controllers
{
    [Route("/v1/perfil")]
    [ApiController]
    public class PerfilController : ControllerBase<PerfilController>
    {
        readonly ILogger<PerfilController> _logger;
        readonly Unit.Application.Sevices.IPerfilService _Service;
        readonly IUsuarioService _usuarioService;

        public PerfilController(ILogger<PerfilController> logger, Unit.Application.Sevices.IPerfilService service, IUsuarioService usuarioService)
        {
            _logger = logger;
            _Service = service;
            _usuarioService = usuarioService;
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
        public async Task<IActionResult> GetAll([FromQuery] QueryPerfilRequest condicao)
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
        public async Task<IActionResult> CreateUser([FromBody] CreatePerfilRequest command)
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
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdatePerfilRequest command)
        {
            command.Id = id;
            var dados = await _Service.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet("{id}/usuarios")]
        //[Authorize]
        public async Task<IActionResult> GetUsuarios([FromRoute] int id)
        {
            var dados = await _Service.GetUsersByPerfil(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("removerperfil")]
        [Authorize]
        public async Task<IActionResult> RemovePerfil([FromBody] RemovePerfilUsuarioRequest command)
        {
            var dados = await _usuarioService.RemPerfil(command.ID);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("adicionarperfil")]
        [Authorize]
        public async Task<IActionResult> AdicionaPerfil([FromBody] AdicionaPerfilUsuarioRequest command)
        {
            var dados = await _usuarioService.AddPerfil(command.UsuarioId,command.PerfilId);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion       
    }
}
