using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Services
{
    public interface ICongService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(CongQueryModel condicao);
        Task<Reply> AddAsync(CongNewModel novo);
        Task<Reply> UpdateAsync(CongUpdateModel alterado);
    }
}
