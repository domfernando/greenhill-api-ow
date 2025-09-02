using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/relatorio")]
    [ApiController]
    public class RelatorioController : ControllerBase<RelatorioController>
    {
        readonly ILogger<RelatorioController> _logger;
        readonly Unit.Application.Sevices.IRelatorioService _Service;
        readonly IPubService _pubService;

        public RelatorioController(ILogger<RelatorioController> logger, Unit.Application.Sevices.IRelatorioService service, IPubService pubService)
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
        public async Task<IActionResult> GetAll([FromQuery] QueryRelatorioRequest condicao)
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
        public async Task<IActionResult> Create([FromBody] CreateRelatorioRequest command)
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRelatorioRequest command)
        {
            command.Id = id;
            var dados = await _Service.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRelatorio(int id)
        {
            var dados = await _Service.Delete(id);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("batch")]
        [Authorize]
        public async Task<IActionResult> CreateBatch([FromBody] CreateRelatorioBatchRequest command)
        {
            var dados = await _Service.AddBatch(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        [Route("grupo")]
        [Authorize]
        public async Task<IActionResult> CreateByGrupo([FromBody] CreateRelatorioByGrupoRequest command)
        {
            var dados = await _Service.AddByGrupo(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion       
    }
}
