using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IUsuarioService
    {
        Task<Reply> Login(LoginRequest request);
        Task<Reply> GetMfaToken(GetMfaTokenRequest request);
        Task<Reply> LoginMfa(LoginMfaRequest request);
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryUsuarioRequest condicao);
        Task<Reply> Add(CreateUsuarioRequest entidade);
        Task<Reply> Update(UpdateUsuarioRequest entidade);
        Task<bool> EmailExist(string email);
        Task<Reply> Validate(string codigo);
        Task<Reply> AddPerfil(int idUsuario, int idPerfil);
        Task<Reply> RemPerfil(int id);
    }
}
