using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IPerfilService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryPerfilRequest condicao);
        Task<Reply> Add(CreatePerfilRequest entidade);
        Task<Reply> Update(UpdatePerfilRequest entidade);
        Task<Reply> GetUsersByPerfil(int id);
        Task<Reply> GetPerfisByUsuario(int id);
    }
}
