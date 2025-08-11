using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;

namespace Unit.API.Controllers
{
    [Route("/v1/util")]
    [ApiController]
    public class UtilController : ControllerBase<UtilController>
    {
        readonly ILogger<UtilController> _logger;
        readonly IViaCepService _viaCepService;

        public UtilController(ILogger<UtilController> logger, IViaCepService viaCepService)
        {
            _logger = logger;
            _viaCepService = viaCepService;
        }

        #region Correio

        [HttpGet("consulta-cep/{cep}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] string cep)
        {
            var dados = await _viaCepService.ConsultaCep(cep);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }      

        #endregion       
    }
}
