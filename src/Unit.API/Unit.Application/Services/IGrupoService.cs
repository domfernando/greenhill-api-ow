using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IGrupoService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryGrupoRequest condicao);
        Task<Reply> Add(CreateGrupoRequest entidade);
        Task<Reply> Update(UpdateGrupoRequest entidade);
        Task<Reply> GetGrupoPub(int id);
        Task<Reply> UpdateGrupoPub(UpdateGrupoPubRequest entidade);
    }
}
