using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/cong")]
    [ApiController]
    public class CongController : ControllerBase<CongController>
    {
        readonly ILogger<CongController> _logger;
        readonly ICongService _Service;

        public CongController(ILogger<CongController> logger, ICongService service)
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
        //[Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CongQueryModel condicao)
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
        public async Task<IActionResult> Create([FromBody] CongNewModel command)
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
        public async Task<IActionResult> Update(int id, [FromBody] CongUpdateModel command)
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
