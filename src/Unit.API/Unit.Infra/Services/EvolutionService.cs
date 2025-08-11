using Unit.Application.Models;
using Unit.Application.Services;
using Unit.Domain.Entities.Util;
using System.Net;
using System.Text;
using Unit.Application.Util;
using Microsoft.Extensions.Configuration;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;

namespace Unit.Infra.Services
{
    public class EvolutionService : IEvolutionService
    {
        private readonly HttpClient _httpClient;
        private readonly IServicoService _servicoService;
        private ConfigServico ConfigServico;

        public EvolutionService(IHttpClient httpClient, IServicoService servicoService)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 // Define a versão mínima do TLS
            };

            _httpClient = new HttpClient(handler);
            _servicoService = servicoService;

            this.ConfigServico = _servicoService.ConfigServico("Evolution").Result;
        }
        public async Task<HttpResponseMessage> SendMessageAsync(SendEvolutionMessageRequest message)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Apikey", this.ConfigServico.Token);

            var body = new
            {
                instance = message.Instance,
                number = message.Number,
                textMessage = new
                {
                    text = message.TextMessage.Text
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{this.ConfigServico.Url}/message/sendText/{message.Instance}", content);

            return response;
        }

        public async Task<Reply> SendMessage(SendEvolutionMessageRequest message)
        {
            Reply retorno = new Reply();

            try
            {
                var response = await this.SendMessageAsync(message);
                retorno.Status = response.StatusCode;
                retorno.Data = response;

                if (response.StatusCode== HttpStatusCode.OK || response.StatusCode== HttpStatusCode.Created)
                {
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

        public async Task<HttpResponseMessage> SendMessageWithLocationAsync(SendEvolutionMessageWithLocationRequest message)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Apikey", this.ConfigServico.Token);

            var json = System.Text.Json.JsonSerializer.Serialize(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{this.ConfigServico.Url}/message/sendLocation/{message.Instance}", content);

            return response;
        }
    }
}
