using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Services
{
    public interface IOradorTemaService
    {
        Task<Reply> GetById(int id);
        Task<Reply> GetAll(OradorTemaQueryRequest condicao);
        Task<Reply> AddAsync(OradorTemaNewRequest novo);
        Task<bool> RemoveAsync(int id);
        Task<Reply> UpdateAsync(OradorTemaUpdateRequest alterado);
    }
}
