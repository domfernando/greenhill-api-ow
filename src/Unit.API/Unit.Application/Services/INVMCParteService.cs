using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Services
{
    public interface INVMCParteService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryNVMCParteRequest condicao);
        Task<Reply> Add(NVMCParte entidade);
        Task<Reply> Update(UpdateNVMCParteRequest entidade);   
    }
}
