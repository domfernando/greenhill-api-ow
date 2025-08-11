using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Config;

namespace Unit.API.Controllers
{
    [Route("/v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase<UsuarioController>
    {
        readonly ILogger<UsuarioController> _logger;
        readonly Unit.Application.Sevices.IUsuarioService _usuarioService;
        readonly IPerfilService _perfilService;

        public UsuarioController(ILogger<UsuarioController> logger, Unit.Application.Sevices.IUsuarioService usuarioService, IPerfilService service)
        {
            _logger = logger;
            _usuarioService = usuarioService;
            _perfilService = service;
        }

        #region CRUD

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var dados = await _usuarioService.GetOne(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryUsuarioRequest condicao)
        {
            var dados = await _usuarioService.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUsuarioRequest command)
        {
            var dados = await _usuarioService.Add(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UpdateUsuarioRequest command)
        {
            command.Id = id;
            var dados = await _usuarioService.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion

        #region Login

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Application.DTOs.Request.LoginRequest command)
        {
            var login = await _usuarioService.Login(command);

            if (!login.Success)
            {
                return BadRequest(login);
            }

            return Ok(login);
        }

        [HttpGet]
        [Route("Token")]
        [AllowAnonymous]
        public async Task<IActionResult> Token([FromQuery] Application.DTOs.Request.GetMfaTokenRequest command)
        {
            var token = await _usuarioService.GetMfaToken(command);

            if (!token.Success)
            {
                return BadRequest(token);
            }

            return Ok(token);
        }

        [HttpPost]
        [Route("Login-mfa")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginMfa([FromBody] Application.DTOs.Request.LoginMfaRequest command)
        {
            var login = await _usuarioService.LoginMfa(command);

            if (!login.Success)
            {
                return BadRequest(login);
            }

            return Ok(login);
        }

        [HttpGet("validar")]
        [AllowAnonymous]
        public async Task<IActionResult> Validar(string codigo)
        {
            var reply = await _usuarioService.Validate(codigo);

            if (!reply.Success)
            {
                return BadRequest(reply);
            }
            return Ok(reply);
        }

        #endregion

        #region Perfis

        [HttpGet("{id}/perfis")]
        //[Authorize]
        public async Task<IActionResult> GetPerfis([FromRoute] int id)
        {
            var dados = await _perfilService.GetPerfisByUsuario(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion
    }
}
