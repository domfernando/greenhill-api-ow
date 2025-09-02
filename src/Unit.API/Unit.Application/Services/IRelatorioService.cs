using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IRelatorioService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryRelatorioRequest condicao);
        Task<Reply> Add(CreateRelatorioRequest entidade);
        Task<Reply> AddBatch(CreateRelatorioBatchRequest entidade);
        Task<Reply> AddByGrupo(CreateRelatorioByGrupoRequest entidade);
        Task<Reply> Update(UpdateRelatorioRequest entidade);   
        Task<Reply> Delete(int id);
    }
}
