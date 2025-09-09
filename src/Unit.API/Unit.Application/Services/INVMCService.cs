using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface INVMCService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryPapelRequest condicao);
        Task<Reply> Add(CreatePapelRequest entidade);
        Task<Reply> Update(UpdatePapelRequest entidade);   
    }
}
