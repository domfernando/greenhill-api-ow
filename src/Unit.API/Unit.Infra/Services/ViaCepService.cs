using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;

namespace Unit.Infra.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly IServicoService _servicoService;
        private ConfigServico ConfigServico;

        public ViaCepService(IHttpClient httpClient, IServicoService servicoService)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 // Define a versão mínima do TLS
            };
            _httpClient = new HttpClient(handler);
            _servicoService = servicoService;
            this.ConfigServico = _servicoService.ConfigServico("Cep").Result;
        }
        public async Task<Reply> ConsultaCep(string cep)
        {
            Reply retorno = new Reply();
            string _cep = cep.Replace("-", "").Replace(".", "").Trim();

            try
            {
                var response = await _httpClient.GetAsync($"{this.ConfigServico.Url}/{_cep}/json");

                retorno.Status = response.StatusCode;               

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    var resp = await response.Content.ReadAsStringAsync();

                    var jSon = JsonConvert.DeserializeObject<ViaCepResponse>(resp);

                    retorno.Data = jSon;
                    retorno.Messages.Add("Enviada com sucesso");
                }
                else
                {
                    retorno.Messages.Add("Ocorreram erros.");
                }
            }
            catch (Exception ex)
            {
                retorno.Messages.Add("Ocorreram erros.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
