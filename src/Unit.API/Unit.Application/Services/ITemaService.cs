using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Services
{
    public interface ITemaService
    {
        Task<Reply> GetById(int id);
        Task<Reply> AddAsync(TemaNewRequest novo);
        Task<Reply> UpdateAsync(TemaUpdateRequest alterado);
        Task<Reply> GetAll(TemaQueryRequest condicao);
    }
}
