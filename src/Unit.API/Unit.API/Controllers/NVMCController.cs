using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Globalization;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;
using HtmlAgilityPack;

namespace Unit.API.Controllers
{
    [Route("/v1/nvmc")]
    [ApiController]
    public class NVMCController : ControllerBase<NVMCController>
    {
        readonly ILogger<NVMCController> _logger;

        public NVMCController(ILogger<NVMCController> logger)
        {
            _logger = logger;
        }

        #region Webscrap Apostila

        [HttpGet("scrap")]
        //[Authorize]
        public async Task<IActionResult> ScrapApostila([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL is required.");

            try
            {
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Encontrar todos os links das semanas
                var weekLinks = doc.DocumentNode.SelectNodes("//a[contains(@href, '/pt/biblioteca/jw-apostila-do-mes/') and contains(@href, '-mwb/')]");
                if (weekLinks == null)
                    return Ok(new List<object>());

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

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao fazer scrap: {ex.Message}");
            }
        }

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
    }

    #endregion
}

