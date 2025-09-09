using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Util;

namespace Unit.Infra.Services
{
    public class MySocketService : Hub
    {
        private readonly IHubContext<Hub> _hubContext;
        public MySocketService(IHubContext<Hub> hubContext)
        {
            _hubContext = hubContext;
        }
        //public async Task NotificacaoUsuario(string usuario, string mensagem)
        //{
        //    await _hubContext.Clients.User(usuario).SendAsync("Notificar", usuario, mensagem);
        //}
        //public override async Task OnConnectedAsync()
        //{
        //    Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception? exception)
        //{
        //    Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
        //    await base.OnDisconnectedAsync(exception);
        //}

        public async Task<Reply> SendNotificacao(ComNotificarRequest comando)
        {
            // Pseudocode:
            // 1. Create an HttpClient instance.
            // 2. Build the request to the endpoint ComController.Notificar.
            // 3. Send the POST request with the ComNotificarRequest payload.
            // 4. Return the result as IActionResult.
            Reply retorno = new Reply();
            try
            {
                using var httpClient = new HttpClient();
                var url = "https://<your-api-base-url>/comm/notificar"; // Replace with actual base URL

                var response = await httpClient.PostAsJsonAsync(url, comando);

                if (response.IsSuccessStatusCode)
                {
                    retorno.Success = true;
                    retorno.Messages.Add("Notificação enviada com sucesso.");
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Não foi possível enviar a notificação.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add($"Não foi possível enviar a notificação. {ex.Message}");
            }

            return retorno;
        }
    }
}
