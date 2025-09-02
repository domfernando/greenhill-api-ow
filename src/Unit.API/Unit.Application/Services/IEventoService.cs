using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IEventoService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryEventoRequest condicao);
        Task<Reply> GetByPapel(QueryEventoByPapelRequest condicao);
        Task<Reply> Add(CreateEventoRequest entidade);
        Task<Reply> Update(UpdateEventoRequest entidade);
        Task<Reply> AddPapel(int pubId, int papelId);
        Task<Reply> RemPapel(int id);
    }
}
