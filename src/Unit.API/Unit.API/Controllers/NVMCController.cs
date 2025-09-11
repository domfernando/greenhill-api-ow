using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using System.Text.Json;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;
using Unit.Infra.Services;

namespace Unit.API.Controllers
{
    [Route("/v1/nvmc")]
    [ApiController]
    public class NVMCController : ControllerBase<NVMCController>
    {
        readonly ILogger<NVMCController> _logger;
        private readonly IHubContext<MySocketService> _hubContext;
        private readonly IWebHostEnvironment _env;
        private readonly INVMCService _nmvcService;
        private readonly INVMCParteService _nvmcParteService;
        public NVMCController(ILogger<NVMCController> logger, IHubContext<MySocketService> hubContext, IWebHostEnvironment env, INVMCService nmvcService, INVMCParteService nvmcParteService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _env = env;
            _nmvcService = nmvcService;
            _nvmcParteService = nvmcParteService;
        }

        #region Webscrap Apostila

        [HttpGet("scrap")]
        [Authorize]
        public async Task<IActionResult> ScrapApostila([FromQuery] string url)
        {
            Reply retorno = new Reply();

            try
            {
                var usuario = User?.Identity.Name;

                if (_env.IsDevelopment())
                {
                    if (!string.IsNullOrEmpty(usuario))
                    {
                        await _hubContext.Clients.All.SendAsync("Push", usuario, $"A geração dos conteúdos foi iniciada. Vocês será avisado no final do processo.");
                    }

                    retorno = await this.ScrapApostila(url, usuario);
                }
                else
                {
                    if (!string.IsNullOrEmpty(usuario))
                    {
                        await _hubContext.Clients.All.SendAsync("Push", usuario, $"A geração dos conteúdos foi agendada. Vocês será avisado no final do processo.");
                    }

                    string jobId;
                    jobId = BackgroundJob.Schedule(() => this.ScrapApostila(url, usuario), TimeSpan.FromMinutes(1));

                    retorno.Success = true;
                    retorno.Messages.Add("Processo em segundo plano iniciado. Job ID: " + jobId);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível gerar o conteúdo.");
                retorno.Errors.Add(ex.Message);
            }

            return Ok(retorno);
        }

        #endregion

        #region CRUD

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var dados = await _nmvcService.GetOne(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryNVMCRequest condicao)
        {
            var dados = await _nmvcService.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> Create([FromBody] CreateNVMCRequest command)
        //{
        //    var dados = await _nmvcService.Add(command);
        //    if (!dados.Success)
        //    {
        //        return BadRequest(dados);
        //    }

        //    return Ok(dados);
        //}

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNVMCRequest command)
        {
            command.Id = id;
            var dados = await _nmvcService.Update(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion

        #region Metodos Privados

        private string GetThursdayFromRange(string rangeText)
        {
            // Exemplo: "2–8 de setembro"
            // Objetivo: retornar a data de quinta-feira do intervalo

            try
            {
                var ptBR = new CultureInfo("pt-BR");
                var diasSemana = new[] { "domingo", "segunda-feira", "terça-feira", "quarta-feira", "quinta-feira", "sexta-feira", "sábado" };
                var meses = ptBR.DateTimeFormat.MonthNames.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                // Separar datas
                var dashIdx = rangeText.IndexOf('–');
                if (dashIdx == -1) dashIdx = rangeText.IndexOf('-');
                if (dashIdx == -1) return "";

                var startText = rangeText.Substring(0, dashIdx).Trim();
                var endText = rangeText.Substring(dashIdx + 1).Trim();

                // Encontrar mês
                var month = meses.FirstOrDefault(m => endText.Contains(m));
                if (month == null) return "";

                // Encontrar ano
                var ano = DateTime.Now.Year;
                var yearMatch = System.Text.RegularExpressions.Regex.Match(endText, @"\d{4}");
                if (yearMatch.Success)
                    ano = int.Parse(yearMatch.Value);

                // Encontrar dias
                var startDayMatch = System.Text.RegularExpressions.Regex.Match(startText, @"\d+");
                var endDayMatch = System.Text.RegularExpressions.Regex.Match(endText, @"\d+");

                if (!startDayMatch.Success || !endDayMatch.Success)
                    return "";

                var startDay = int.Parse(startDayMatch.Value);
                var endDay = int.Parse(endDayMatch.Value);

                var monthIdx = Array.IndexOf(meses, month) + 1;
                if (monthIdx == 0) return "";

                var startDate = new DateTime(ano, monthIdx, startDay);
                var endDate = new DateTime(ano, monthIdx, endDay);

                // Procurar quinta-feira no intervalo
                for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    if (dt.DayOfWeek == DayOfWeek.Thursday)
                        return dt.ToString("yyyy-MM-dd");
                }
            }
            catch
            {
                // Ignorar erros de parsing
            }
            return "";
        }

        private async Task<Reply> ScrapApostila(string url, string usuario)
        {
            Reply retorno = new Reply();

            if (string.IsNullOrEmpty(url))
            {
                retorno.Success = false;
                retorno.Messages.Add("URL is required.");
                return retorno;
            }

            try
            {
                retorno.Messages.Add($"[{System.DateTime.Now}] Iniciando a geração do conteúdo das reuniões.");

                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Encontrar todos os links das semanas
                var weekLinks = doc.DocumentNode.SelectNodes("//a[contains(@href, '/pt/biblioteca/jw-apostila-do-mes/') and contains(@href, '-mwb/')]");
                if (weekLinks == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Nenhum link de semana encontrado.");
                    retorno.Data = new List<object>();
                    return retorno;
                }

                var results = new List<object>();

                foreach (var link in weekLinks)
                {
                    var weekUrl = link.GetAttributeValue("href", "");
                    if (!weekUrl.StartsWith("http"))
                        weekUrl = "https://www.jw.org" + weekUrl;

                    // Scraping da página da semana
                    var weekHtml = await httpClient.GetStringAsync(weekUrl);
                    var weekDoc = new HtmlDocument();
                    weekDoc.LoadHtml(weekHtml);

                    // Tentar encontrar o intervalo de datas na página
                    var dateNode = weekDoc.DocumentNode
                        .SelectSingleNode("//h2[contains(@class, 'week-title')]") ??
                        weekDoc.DocumentNode.SelectSingleNode("//h1");

                    string quintaFeira = "";
                    if (dateNode != null)
                    {
                        var dateRangeText = dateNode.InnerText.Trim();
                        quintaFeira = GetThursdayFromRange(dateRangeText);
                    }

                    // Encontrar todas as seções
                    //var sectionNodes = weekDoc.DocumentNode.SelectNodes("//div[contains(@class, 'section') or contains(@class, 'study-text-block')]");
                    var sectionNodes = weekDoc.DocumentNode.SelectNodes("//div[contains(@class, 'bodyTxt')]");

                    var secoes = new List<object>();
                    string nomeSecaoAtual = "";
                    string tituloParte = "";

                    if (sectionNodes != null)
                    {
                        var reuniaoSemana = new object();
                        var partes = new List<object>();

                        foreach (var section in sectionNodes)
                        {
                            var titulos = section.SelectNodes("//h2 | //h3");

                            foreach (var titulo in titulos)
                            {
                                var _atributos = titulo.ParentNode.Attributes;

                                var _classes = _atributos.Where(x => x.Name == "class").Select(v => v.Value).ToList();

                                string[] ignorarClasses = new string[]
                                {
                                    "",
                                    "siteNameContainer",
                                    "navLinkNext"
                                };

                                var ignorar = _classes.Count == 0 || _classes.Any(c => ignorarClasses.Contains(c));

                                if (ignorar)
                                    continue;

                                var _ehSecao = _atributos.FirstOrDefault(x => x.Name == "class" && x.Value.Contains("dc-icon")) != null;
                                if (_ehSecao)
                                {
                                    nomeSecaoAtual = titulo.InnerText.Trim();

                                    reuniaoSemana = new
                                    {
                                        mes = dateNode?.InnerText.Trim() ?? "",
                                        data = quintaFeira,
                                        partes = new List<object>()
                                    };
                                }
                                else
                                {
                                    tituloParte = titulo.InnerText.Trim();
                                    var detalhesParte = "";

                                    if (titulo.Name == "h3")
                                    {
                                        string[] ignorarClassesDivFinal = new string[]
                                          {
                                                "navLinkNext",
                                                "articleNavLinks"
                                          };
                                        // Substitua o trecho selecionado por este código para encontrar o próximo node após o título e recuperar o texto
                                        if (titulo != null)
                                        {
                                            var nextNode = titulo.NextSibling;
                                            while (nextNode != null && nextNode.NodeType != HtmlNodeType.Element)
                                            {
                                                nextNode = nextNode.NextSibling;
                                            }
                                            if (nextNode != null && nextNode.Name == "div" &&
                                                nextNode.Attributes["class"] != null &&
                                                !ignorarClassesDivFinal.Contains(nextNode.Attributes["class"].Value))
                                            {
                                                var detalhes = nextNode.InnerText.Trim();

                                                if (nomeSecaoAtual.ToUpper() == "TESOUROS DA PALAVRA DE DEUS")
                                                {
                                                    detalhesParte = $"{detalhes.Split(")")[0]})";
                                                }
                                                else
                                                {
                                                    detalhesParte = detalhes;
                                                }
                                            }
                                        }
                                    }

                                    partes.Add(new
                                    {
                                        titulo = tituloParte,
                                        descricao = detalhesParte,
                                        designado = "",
                                        ajudante = "",
                                        nomeSecao = nomeSecaoAtual
                                    });
                                }
                            }
                            // Depois (correto):
                            results.Add(new
                            {
                                mes = dateNode?.InnerText.Trim() ?? "",
                                data = quintaFeira,
                                partes = partes
                            });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(usuario))
                {
                    await _hubContext.Clients.All.SendAsync("Push", usuario, $"Conteúdo capturado. Iniciando a criação dos dados.");
                }

                var dadosApostila = JsonSerializer.Deserialize<NVMCApostilaRequest>(JsonSerializer.Serialize(results));

                var resultadoDados = await this.CreateNVMCBatch(dadosApostila);

                retorno.Success = true;
                retorno.Messages.Add($"[{System.DateTime.Now}] Criação dos dados concluída.");
                retorno.Messages.AddRange(resultadoDados.Messages);
                retorno.Messages.Add($"[{System.DateTime.Now}] A geração do conteúdo das reuniões foi concluída com sucesso.");
                retorno.Data = results;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível gerar o conteúdo.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        private async Task<Reply> CreateNVMCBatch(NVMCApostilaRequest dados)
        {
            Reply retorno = new Reply();
            List<Domain.Entities.Cadastro.NVMC> lista = new List<Domain.Entities.Cadastro.NVMC>();
            String[] PRESIDENCIA = ["Presidente", "OracaoInicial", "OracaoFinal"];

            try
            {
                if (ModelState.IsValid == false)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Não foi possível realizar a operação");
                    return retorno;
                }

                foreach (var item in dados.Semanas)
                {
                    var options = new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    var _conteudo = System.Text.Json.JsonSerializer.Serialize(item, options);

                    #region Criando semana NVMC

                    var novo = await _nmvcService.Add(new Domain.Entities.Cadastro.NVMC()
                    {
                        Data = item.Data,
                        Mes = item.Data,
                        Presidente = item.Presidente,
                        OracaoInicial = item.OracaoInicial,
                        OracaoFinal = item.OracaoFinal,
                        Conteudo = _conteudo
                    });

                    #endregion

                    var novaSemana = (NVMC)novo.Data;

                    if (novaSemana != null && novaSemana.ID > 0)
                    {
                        foreach (var presidencia in PRESIDENCIA)
                        {
                            var novaParte = _nvmcParteService.Add(new Domain.Entities.Cadastro.NVMCParte()
                            {
                                NVMCID = novaSemana.ID,
                                Titulo = presidencia,
                                Descricao = "",
                                NomeSecao = "Presidencia",
                                Privilegio = presidencia == "Presidente" ? "DIANTEIRA" : "EXEMPLAR"
                            }).Result;
                        }

                        foreach (var secao in item.Secoes)
                        {
                            foreach (var parte in secao.Partes)
                            {
                                var novaParte = _nvmcParteService.Add(new NVMCParte()
                                {
                                    NVMCID = novaSemana.ID,
                                    Titulo = parte.Titulo,
                                    Descricao = parte.Descricao,
                                    NomeSecao = secao.Titulo,
                                    Privilegio = parte.Privilegio
                                }).Result;
                            }
                        }

                        lista.Add(novaSemana);

                        retorno.Success = true;
                        retorno.Messages.Add($"{novaSemana} OK.");
                        retorno.Data = lista;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível realizar a operação");
            }

            return retorno;
        }

        #endregion
    }
}

