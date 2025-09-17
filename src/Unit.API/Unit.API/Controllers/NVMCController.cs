using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
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
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryNVMCRequest condicao)
        {
            var dados = await _nmvcService.GetAll(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet("lista")]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] QueryNVMCRequest condicao)
        {
            var dados = await _nmvcService.GetList(condicao);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

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

        [HttpGet("parte/{id}")]
        [Authorize]
        public async Task<IActionResult> GetParte([FromRoute] int id)
        {
            var dados = await _nmvcService.GetOneParte(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpPut("parte/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateParte(int id, [FromBody] UpdateNVMCParteRequest command)
        {
            command.Id = id;
            var dados = await _nmvcService.UpdateParte(command);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet("partes/pub/{id}")]
        [Authorize]
        public async Task<IActionResult> GetHistorico([FromRoute] int id)
        {
            var dados = await _nmvcService.GetPartesByPub(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        [HttpGet("partes/grafico/pub/{id}")]
        [Authorize]
        public async Task<IActionResult> GetGraficosPub([FromRoute] int id)
        {
            var dados = await _nmvcService.GetGraficoPartesByPub(id);
            if (!dados.Success)
            {
                return BadRequest(dados);
            }

            return Ok(dados);
        }

        #endregion

        #region Scrap
        private string GetThursdayFromRange(string rangeText)
        {
            // Exemplo: "2–8 de setembro" ou "28 de julho–3 de agosto"
            // Objetivo: retornar a data de quinta-feira do intervalo

            try
            {
                var ptBR = new CultureInfo("pt-BR");
                var meses = ptBR.DateTimeFormat.MonthNames.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                // Separar datas
                var dashIdx = rangeText.IndexOf('–');
                if (dashIdx == -1) dashIdx = rangeText.IndexOf('-');
                if (dashIdx == -1) return "";

                var startText = rangeText.Substring(0, dashIdx).Trim();
                var endText = rangeText.Substring(dashIdx + 1).Trim();

                // Encontrar mês e ano do início e do fim
                string startMonth = null, endMonth = null;
                int ano = DateTime.Now.Year;

                // Tenta encontrar mês no início
                foreach (var mes in meses)
                {
                    if (!string.IsNullOrEmpty(mes) && startText.Contains(mes))
                    {
                        startMonth = mes;
                        break;
                    }
                }
                // Tenta encontrar mês no fim
                foreach (var mes in meses)
                {
                    if (!string.IsNullOrEmpty(mes) && endText.Contains(mes))
                    {
                        endMonth = mes;
                        break;
                    }
                }

                // Se não encontrou mês no início, assume o mês do fim
                if (startMonth == null) startMonth = endMonth;
                if (endMonth == null) endMonth = startMonth;

                // Encontrar ano (se houver)
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

                var startMonthIdx = Array.IndexOf(meses, startMonth) + 1;
                var endMonthIdx = Array.IndexOf(meses, endMonth) + 1;
                if (startMonthIdx == 0 || endMonthIdx == 0) return "";

                var startDate = new DateTime(ano, startMonthIdx, startDay);
                var endDate = new DateTime(ano, endMonthIdx, endDay);

                // Se o intervalo cruzar o ano (ex: dezembro–janeiro)
                if (endDate < startDate)
                {
                    endDate = endDate.AddYears(1);
                }

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

        //private async Task<Reply> ScrapApostila(string url, string usuario)
        //{
        //    Reply retorno = new Reply();

        //    try
        //    {
        //        var httpClient = new HttpClient();
        //        var html = await httpClient.GetStringAsync(url);

        //        var doc = new HtmlDocument();
        //        doc.LoadHtml(html);

        //        var apostila = new NVMCApostilaRequest();

        //        var weekLinks = doc.DocumentNode.SelectNodes("//a[contains(@href, '/pt/biblioteca/jw-apostila-do-mes/') and contains(@href, '-mwb/')]");
        //        if (weekLinks == null)
        //        {
        //            retorno.Success = false;
        //            retorno.Data = apostila;
        //            return retorno;
        //        }

        //        foreach (var link in weekLinks)
        //        {
        //            var weekUrl = link.GetAttributeValue("href", "");
        //            if (!weekUrl.StartsWith("http"))
        //                weekUrl = "https://www.jw.org" + weekUrl;

        //            var weekHtml = await httpClient.GetStringAsync(weekUrl);
        //            var weekDoc = new HtmlDocument();
        //            weekDoc.LoadHtml(weekHtml);

        //            var dateNode = weekDoc.DocumentNode
        //                .SelectSingleNode("//h2[contains(@class, 'week-title')]") ??
        //                weekDoc.DocumentNode.SelectSingleNode("//h1");

        //            var semana = new NVMCSemanaRequest();
        //            if (dateNode != null)
        //            {
        //                var dateRangeText = dateNode.InnerText.Trim();
        //                // Aqui você pode converter para DateTime conforme sua lógica
        //                semana.Data = DateTime.Parse(dateRangeText); // Ajuste conforme necessário
        //            }

        //            var sectionNodes = weekDoc.DocumentNode.SelectNodes("//div[contains(@class, 'section') or contains(@class, 'study-text-block')]");
        //            if (sectionNodes != null)
        //            {
        //                foreach (var section in sectionNodes)
        //                {
        //                    var secao = new NVMCSecaoRequest();
        //                    secao.Titulo = section.SelectSingleNode(".//h3")?.InnerText.Trim()
        //                        ?? section.SelectSingleNode(".//h2[contains(@class, 'du-fontSize--base')]")?.InnerText.Trim()
        //                        ?? "";

        //                    var parteNodes = section.SelectNodes(".//li[contains(@class, 'study-item')] | .//div[contains(@class, 'study-parts')]/div");
        //                    if (parteNodes != null)
        //                    {
        //                        foreach (var parte in parteNodes)
        //                        {
        //                            var parteRequest = new NVMCParteRequest
        //                            {
        //                                Titulo = parte.SelectSingleNode(".//span[contains(@class, 'title')]")?.InnerText.Trim()
        //                                    ?? parte.SelectSingleNode(".//strong")?.InnerText.Trim()
        //                                    ?? parte.InnerText.Trim(),
        //                                NomeSecao = secao.Titulo
        //                            };
        //                            secao.Partes.Add(parteRequest);
        //                        }
        //                    }
        //                    semana.Secoes.Add(secao);
        //                }
        //            }
        //            apostila.Semanas.Add(semana);
        //        }

        //        retorno.Success = true;
        //        retorno.Data = apostila;

        //        if (!string.IsNullOrEmpty(usuario))
        //        {
        //            await _hubContext.Clients.All.SendAsync("Push", usuario, $"Conteúdo capturado. Iniciando a criação dos dados.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.Success = false;
        //        retorno.Messages.Add("Não foi possível carregar");
        //        retorno.Errors.Add(ex.Message);
        //    }

        //    return retorno;
        //}

        private async Task<Reply> ScrapApostila(string url, string usuario)
        {
            Reply retorno = new Reply();
            string semana = string.Empty;

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

                var results = new NVMCApostilaRequest();
                results.Partes = new List<NVMCParteRequest>();

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
                        semana = dateRangeText;
                        quintaFeira = GetThursdayFromRange(dateRangeText);
                    }

                    // Encontrar todas as seções
                    //var sectionNodes = weekDoc.DocumentNode.SelectNodes("//div[contains(@class, 'section') or contains(@class, 'study-text-block')]");
                    var sectionNodes = weekDoc.DocumentNode.SelectNodes("//div[contains(@class, 'bodyTxt')]");

                    var secoes = new List<NVMCSecaoRequest>();
                    string nomeSecaoVigente = "";
                    string nomeSecaoAtual = "";
                    string tituloParte = "";
                    NVMCSecaoRequest secaoAtual = new NVMCSecaoRequest();

                    if (sectionNodes != null)
                    {
                        secaoAtual.Partes = new List<NVMCParteRequest>();

                        var reuniaoSemana = new NVMCSemanaRequest()
                        {
                            Mes = dateNode?.InnerText.Trim() ?? "",
                            Data = !string.IsNullOrEmpty(quintaFeira)
                                            ? DateTime.ParseExact(quintaFeira, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                            : default(DateTime),
                            Secoes = new List<NVMCSecaoRequest>()
                        };

                        var partes = new List<NVMCParteRequest>();

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

                                if (string.IsNullOrEmpty(nomeSecaoAtual) && string.IsNullOrEmpty(nomeSecaoVigente))
                                {
                                    secaoAtual.Titulo = "Presidencia";
                                }

                                var _ehSecao = _atributos.FirstOrDefault(x => x.Name == "class" && x.Value.Contains("dc-icon")) != null;
                                if (_ehSecao)
                                {
                                    nomeSecaoAtual = titulo.InnerText.Trim();
                                    secaoAtual.Titulo = nomeSecaoAtual;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(nomeSecaoAtual))
                                    {
                                        secaoAtual.Titulo = "Presidencia";
                                    }

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

                                    results.Partes.Add(new NVMCParteRequest
                                    {
                                        Data = !string.IsNullOrEmpty(quintaFeira)
                                            ? DateTime.ParseExact(quintaFeira, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                            : default(DateTime),
                                        Titulo = tituloParte,
                                        Descricao = detalhesParte,
                                        Designado = "",
                                        Ajudante = "",
                                        NomeSecao = nomeSecaoAtual,
                                        Semana = semana
                                    });
                                }
                            }
                        }

                        //reuniaoSemana.Secoes.Add(secaoAtual);
                        //results.Semanas.Add(reuniaoSemana);
                    }
                }

                if (!string.IsNullOrEmpty(usuario))
                {
                    await _hubContext.Clients.All.SendAsync("Push", usuario, $"Conteúdo capturado. Iniciando a criação dos dados.");
                }

                var resultadoDados = await this.CreateNVMCBatch(results);

                retorno.Success = true;
                retorno.Messages.Add($"[{System.DateTime.Now}] Criação dos dados concluída.");
                //retorno.Messages.AddRange(resultadoDados.Messages);
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
            String[] PRESIDENCIA = ["Presidente", "Oração Inicial", "Oração Final"];
            List<NVMC> apostila = new List<NVMC>();
            int id = 1;
            int idParte = 1;
            string tituloParte = "";
            string nomeSecao = "";
            string privilegio = "";
            string descricao = "";

            try
            {
                if (ModelState.IsValid == false)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Não foi possível realizar a operação");
                    return retorno;
                }

                // Por este agrupamento por Data:
                var semanasAgrupadas = dados.Partes
                                            .GroupBy(s => s.Data)
                                            .Select(g => new
                                            {
                                                Data = g.Key,
                                                Partes = g.ToList()
                                            })
                                            .Distinct().ToList();

                foreach (var item in semanasAgrupadas)
                {
                    if (item.Data == null || item.Data == default(DateTime) || item.Data == DateTime.MinValue)
                    {
                        continue;
                    }

                    var registros = item.Partes.Take(12);

                    var nova = await _nmvcService.Add(new Domain.Entities.Cadastro.NVMC()
                    {
                        Data = item.Data,
                        Mes = item.Data,
                        Semana = item.Partes.FirstOrDefault().Semana,
                        Presidente = "",
                        OracaoInicial = "",
                        OracaoFinal = "",
                        Conteudo = ""
                    });

                    var novaSemana = (NVMC)nova.Data;

                    if (novaSemana != null && novaSemana.ID > 0)
                    {
                        var presidencia = _nvmcParteService.Add(new Domain.Entities.Cadastro.NVMCParte()
                        {
                            NVMCID = novaSemana.ID,
                            Titulo = "Presidente",
                            Descricao = "",
                            NomeSecao = "Presidencia",
                            Privilegio = "PVM"
                        }).Result;


                        foreach (var registro in registros)
                        {
                            if (registro.Titulo.ToLower().Contains("veja mais on-line"))
                            {
                                continue;
                            }

                            if (registro.Titulo.ToLower().Contains("comentários iniciais"))
                            {
                                nomeSecao = "Presidencia";
                                tituloParte = "Oração Inicial";
                                privilegio = "EXEMPLAR";
                                descricao = "";
                            }
                            else if (registro.Titulo.ToLower().Contains("comentários finais"))
                            {
                                nomeSecao = "Presidencia";
                                tituloParte = "Oração Final";
                                privilegio = "EXEMPLAR";
                                descricao = "";
                            }
                            else if (registro.Titulo.ToLower().Contains("3. leitura"))
                            {
                                nomeSecao = registro.NomeSecao;
                                tituloParte = registro.Titulo;
                                privilegio = "HOMEM";
                                descricao =registro.Descricao;
                            }
                            else
                            {
                                tituloParte = registro.Titulo;
                                nomeSecao = registro.NomeSecao;
                                privilegio = registro.Privilegio;
                                descricao = registro.Descricao;
                            }

                            if (tituloParte.ToLower().Contains("cântico"))
                                continue;

                            var novaParte = _nvmcParteService.Add(new Domain.Entities.Cadastro.NVMCParte()
                            {
                                NVMCID = novaSemana.ID,
                                Titulo = tituloParte,
                                Descricao = descricao,
                                NomeSecao = nomeSecao,
                                Privilegio = privilegio
                            }).Result;

                            if (registro.Titulo.ToLower().Contains("comentários finais"))
                            {
                                break;
                            }
                        }
                    }
                }

                #region Old
                //foreach (var item in dados.Semanas)
                //{
                //    var options = new JsonSerializerOptions
                //    {
                //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                //    };

                //    var _conteudo = System.Text.Json.JsonSerializer.Serialize(item, options);

                //    #region Criando semana NVMC

                //    var novo = await _nmvcService.Add(new Domain.Entities.Cadastro.NVMC()
                //    {
                //        Data = item.Data,
                //        Mes = item.Data,
                //        Presidente = item.Presidente,
                //        OracaoInicial = item.OracaoInicial,
                //        OracaoFinal = item.OracaoFinal,
                //        Conteudo = _conteudo
                //    });

                //    #endregion

                //    var novaSemana = (NVMC)novo.Data;

                //    if (novaSemana != null && novaSemana.ID > 0)
                //    {
                //        foreach (var presidencia in PRESIDENCIA)
                //        {
                //            var novaParte = _nvmcParteService.Add(new Domain.Entities.Cadastro.NVMCParte()
                //            {
                //                NVMCID = novaSemana.ID,
                //                Titulo = presidencia,
                //                Descricao = "",
                //                NomeSecao = "Presidencia",
                //                Privilegio = presidencia == "Presidente" ? "DIANTEIRA" : "EXEMPLAR"
                //            }).Result;
                //        }

                //        foreach (var secao in item.Secoes)
                //        {
                //            foreach (var parte in secao.Partes)
                //            {
                //                var novaParte = _nvmcParteService.Add(new NVMCParte()
                //                {
                //                    NVMCID = novaSemana.ID,
                //                    Titulo = parte.Titulo,
                //                    Descricao = parte.Descricao,
                //                    NomeSecao = secao.Titulo,
                //                    Privilegio = parte.Privilegio
                //                }).Result;
                //            }
                //        }

                //        lista.Add(novaSemana);

                //        retorno.Success = true;
                //        retorno.Messages.Add($"{novaSemana} OK.");
                //        retorno.Data = lista;
                //    }
                //}
                #endregion

                retorno.Success = true;
                retorno.Messages.Add("Registros adicionados com sucesso.");
                retorno.Data = dados;
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

