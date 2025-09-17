using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Infra.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/relatorio")]
    [ApiController]
    public class RelatorioController : ControllerBase<RelatorioController>
    {
        readonly ILogger<RelatorioController> _logger;
        readonly Unit.Application.Sevices.IRelatorioService _Service;
        readonly IPubService _pubService;
        private readonly IHubContext<MySocketService> _hubContext;
        private readonly IWebHostEnvironment _env;

        public RelatorioController(ILogger<RelatorioController> logger, Unit.Application.Sevices.IRelatorioService service, IPubService pubService, IHubContext<MySocketService> hubContext, IWebHostEnvironment env)
        {
            _logger = logger;
            _Service = service;
            _pubService = pubService;
            _hubContext = hubContext;
            _env = env;
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

        [HttpGet("pub/{id}")]
        [Authorize]
        public async Task<IActionResult> GetByPub([FromRoute] int id)
        {
            var dados = await _Service.GetByPub(id);
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
            Reply retorno = new Reply();
            try
            {
                command.Usuario = User?.Identity.Name;

                if (!string.IsNullOrEmpty(command.Usuario))
                {
                    await _hubContext.Clients.All.SendAsync("Push", command.Usuario, $"A criação dos relatórios foi solicitada. Aguarde o final do processo");
                }

                if(_env.IsDevelopment() || _env.IsEnvironment("Test"))
                {
                    retorno = await _Service.AddBatch(command);
                    retorno.Messages.Add("Processo concluído.");
                }
                else
                {
                    string jobId;
                    jobId = BackgroundJob.Schedule<IRelatorioService>(svc => svc.AddBatch(command), TimeSpan.FromMinutes(1));

                    retorno.Success = true;
                    retorno.Messages.Add("Processo em segundo plano iniciado. Job ID: " + jobId);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao iniciar o processo em segundo plano.");
                retorno.Errors.Add(ex.Message);
            }

            return Ok(retorno);

            //command.Usuario = User?.Identity?.Name;




            //var dados = await _Service.AddBatch(command);
            //if (!dados.Success)
            //{
            //    return BadRequest(dados);
            //}

            //return Ok(dados);
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
