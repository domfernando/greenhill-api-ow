using Unit.Application.DTOs.Request;
using Unit.Application.Models;
using Unit.Application.Util;
using Unit.Domain.Entities.Util;

namespace Unit.Application.Services
{
    public interface IEvolutionService
    {
        public Task<HttpResponseMessage> SendMessageAsync(SendEvolutionMessageRequest message);
        public Task<Reply> SendMessage(SendEvolutionMessageRequest message);
        public Task<HttpResponseMessage> SendMessageWithLocationAsync(SendEvolutionMessageWithLocationRequest message);
    }
}
