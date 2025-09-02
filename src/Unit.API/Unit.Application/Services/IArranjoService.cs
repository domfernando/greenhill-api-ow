using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Services
{
    public interface IArranjoService
    {
        Task<Reply> GetById(int id);
        Task<Reply> GetAll(ArranjoQueryRequest condicao);
        Task<Reply> GetAllByYear(ArranjoQueryRequest condicao, int ano);
        Task<Reply> AddAsync(ArranjoNewRequest novo);
        Task<Reply> UpdateAsync(ArranjoUpdateRequest alterado);
        Task<string> GenerateContent(int id, string tipo);
        Task<string> GenerateNotification(int id, int messageId, string conteudo, bool whatsapp);
        Task<bool> DeleteNotificationById(int id);
    }
}
