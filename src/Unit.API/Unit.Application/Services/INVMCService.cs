using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Services
{
    public interface INVMCService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryNVMCRequest condicao);
        Task<Reply> GetList(QueryNVMCRequest condicao);
        Task<Reply> Add(NVMC entidade);
        Task<Reply> Update(UpdateNVMCRequest entidade);
        Task<Reply> GetOneParte(int id);
        Task<Reply> GetPartesByPub(int pubId);
        Task<Reply> GetGraficoPartesByPub(int pubId);
        Task<Reply> UpdateParte(UpdateNVMCParteRequest entidade);
    }
}
