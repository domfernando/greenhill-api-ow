using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Unit.API.Controllers
{
    [Route("/v1/arranjo")]
    [ApiController]
    public class ArranjoController : ControllerBase<ArranjoController>
    {
        readonly ILogger<ArranjoController> _logger;
        readonly IArranjoService _Service;
        readonly IDiscursoService _DiscursoService;
        readonly IOradorTemaService _OradorTemaService;
        readonly ITemaService _TemaService;
        readonly IPubService _PubService;
        readonly IMapper _mapper;

        public ArranjoController(ILogger<ArranjoController> logger, IArranjoService service, IDiscursoService discursoService, ITemaService temaService, IOradorTemaService oradorTemaService, IPubService pubService, IMapper mapper)
        {
            _logger = logger;
            _Service = service;
            _TemaService = temaService;
            _DiscursoService = discursoService;
            _OradorTemaService = oradorTemaService;
            _PubService = pubService;
            _mapper = mapper;
        }

        #region Arranjo
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            _logger.LogInformation("Get Arranjo by ID: {Id}", id);
            var dados = await _Service.GetById(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ArranjoQueryRequest condicao)
        {
            var dados = await _Service.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArranjo([FromBody] ArranjoNewRequest comando)
        {
            var dados = await _Service.AddAsync(comando);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArranjo(int id, [FromBody] ArranjoUpdateRequest comando)
        {
            comando.ID = id;
            var dados = await _Service.UpdateAsync(comando);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion

        #region Orador

        [HttpPost("orador")]
        public async Task<IActionResult> CreateOrador([FromBody] OradorNewRequest comando)
        {
            _logger.LogInformation("Create Orador: {@Comando}", comando);
            Reply retorno = new Reply();

            try
            {
                var pub = await _PubService.GetOne(comando.PubId);

                if (pub.Success)
                {
                    var varao = (PubResponse)pub.Data;
                    varao.Orador = true;
                    var _update = _mapper.Map<PubUpdateModel>(varao);

                    retorno = await _PubService.UpdateAsync(_update);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Publicador não encontrado.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar orador");
                retorno.Errors.Add(ex.Message);
                return BadRequest(retorno);
            }

            return Ok(retorno);
        }

        [HttpPut("orador/{id}")]
        public async Task<IActionResult> CreateOrador(int id, [FromBody] OradorUpdateRequest comando)
        {
            _logger.LogInformation("Update Orador: {@Comando}", comando);
            Reply retorno = new Reply();

            try
            {
                comando.PubId = id;
                var pub = await _PubService.GetOrador((int)comando.PubId);

                if (pub.Success)
                {
                    var varao = (PubResponse)pub.Data;
                    varao.Orador = comando.Publico;
                    var _update = _mapper.Map<PubUpdateModel>(varao);

                    retorno = await _PubService.UpdateAsync(_update);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Publicador não encontrado.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar orador");
                retorno.Errors.Add(ex.Message);
                return BadRequest(retorno);
            }

            return Ok(retorno);
        }

        [HttpGet("orador/{id}")]
        public async Task<IActionResult> GetOrador(int id)
        {
            var dados = await _PubService.GetOrador(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpGet("oradores")]
        public async Task<IActionResult> GetOradores([FromQuery] OradorQueryRequest comando)
        {
            _logger.LogInformation("Get Oradores with condition: {@Condicao}", comando);

            var dados = await _PubService.GetOradores(comando);// .GetOradores(condicao);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpGet("orador/tema/{id}")]
        public async Task<IActionResult> GetOradorTema(int id)
        {
            _logger.LogInformation("Get OradorTema with condition: {@Condicao}", id);

            var dados = await _OradorTemaService.GetById(id);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpPost("orador/tema")]
        public async Task<IActionResult> AddOradorTema([FromBody] OradorTemaNewRequest comando)
        {
            _logger.LogInformation("Adicionando tema ao Orador: {@Condicao}", comando);

            var dados = await _OradorTemaService.AddAsync(comando);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpDelete("orador/tema/{id}")]
        public async Task<IActionResult> RemoveOradorTema(int id)
        {
            Reply retorno = new Reply();
            _logger.LogInformation("Removendo tema do Orador: {@Condicao}", id);

            try
            {
                var dados = await _OradorTemaService.RemoveAsync(id);

                retorno.Success = dados;
                if (dados)
                {
                    retorno.Messages.Add("Tema removido do orador com sucesso.");
                }
                else
                {
                    retorno.Messages.Add("Não foi possível remover o tema do orador.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao remover tema do orador.");
                retorno.Errors.Add(ex.Message);
                return BadRequest(retorno);
            }

            return Ok(retorno);
        }

        [HttpPut("orador/tema/{id}")]
        public async Task<IActionResult> UpdateOradorTema([FromBody] OradorTemaUpdateRequest comando)
        {
            _logger.LogInformation("Atualizar tema ao Orador: {@Condicao}", comando);

            var dados = await _OradorTemaService.UpdateAsync(comando);

            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        #endregion

        #region Discursos

        [HttpGet("discursos")]
        public async Task<IActionResult> GetDiscursos([FromQuery] DiscursoQueryRequest query)
        {
            var dados = await _DiscursoService.GetDiscursos(query);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpGet("discurso")]
        public async Task<IActionResult> GetDiscurso(int id)
        {
            var dados = await _DiscursoService.GetById(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpPost("discurso")]
        public async Task<IActionResult> CreateDiscurso([FromBody] DiscursoNewRequest comando)
        {
            var dados = await _DiscursoService.AddAsync(comando);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpPut("discurso/{id}")]
        public async Task<IActionResult> UpdateDiscurso(int id, [FromBody] DiscursoUpdateRequest comando)
        {
            var dados = await _DiscursoService.UpdateAsync(comando);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpDelete("discurso/{id}/delete")]
        public async Task<IActionResult> DeleteDiscurso(int id)
        {
            Reply retorno = new Reply();
            _logger.LogInformation("Removendo discurso: {@Condicao}", id);

            try
            {
                var dados = await _DiscursoService.DeleteById(id);

                retorno.Success = dados;
                if (dados)
                {
                    retorno.Messages.Add("Discurso removido com sucesso.");
                }
                else
                {
                    retorno.Messages.Add("Não foi possível remover o discurso.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao remover o discurso.");
                retorno.Errors.Add(ex.Message);
                return BadRequest(retorno);
            }

            return Ok(retorno);
        }

        #endregion

        #region Tema
        [HttpGet("tema")]
        public async Task<IActionResult> GetTemas([FromQuery] TemaQueryRequest query)
        {
            var dados = await _TemaService.GetAll(query);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpGet("tema/{id}")]
        public async Task<IActionResult> GetTema(int id)
        {
            var dados = await _TemaService.GetById(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        [HttpPost("tema")]
        public async Task<IActionResult> CreateTema([FromBody] TemaNewRequest comando)
        {
            var dados = await _TemaService.AddAsync(comando);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);

        }

        [HttpPut("tema/{id}")]
        public async Task<IActionResult> UpdateTema(int id, [FromBody] TemaUpdateRequest comando)
        {
            var dados = await _TemaService.UpdateAsync(comando);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }
            return Ok(dados);
        }

        #endregion
    }
}
