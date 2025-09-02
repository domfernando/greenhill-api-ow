using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Services
{
    public interface IDiscursoService
    {
        Task<Reply> GetById(int id);
        Task<bool> DeleteById(int id);
        Task<Reply> GetAll(DiscursoQueryRequest condicao);
        Task<Reply> GetDiscursos(DiscursoQueryRequest condicao);
        Task<Reply> GetDiscursosByYear(DiscursoQueryRequest condicao, int ano);
        Task<Reply> AddAsync(DiscursoNewRequest novo);
        Task<Reply> UpdateAsync(DiscursoUpdateRequest alterado);
        Task<string> GenerateNotification(int id, int messageId, string conteudo, bool whatsapp);
        Task<string> GenerateContent(int id);
        Task<bool> DeleteNotificationById(int id);
    }
}
