using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Response;

namespace Unit.Application.Sevices
{
    public interface ITokenService
    {
        Task<string> GenerateToken(UsuarioResponse user);
    }
}
