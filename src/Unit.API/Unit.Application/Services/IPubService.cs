using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Services
{
    public interface IPubService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetList(PubQueryModel condicao);
        Task<Reply> GetAll(PubQueryModel condicao);
        Task<Reply> AddAsync(PubNewModel novo);
        Task<Reply> UpdateAsync(PubUpdateModel alterado);
        Task<Reply> GetOrador(int id);
        Task<Reply> GetOradores(OradorQueryRequest condicao);
        Task<Reply> AddPapel(int pubId, int papelId);
        Task<Reply> RemPapel(int id);
        Task<Reply> RemoverPapel(int pubId, int papelId);
        Task<Reply> AddGrupo(int pubId, int grupoId, string papel);
        Task<Reply> RemGrupo(int id);
        Task<Reply> GetByUsuarioId(int id);
    }
}
