using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Services
{
    public interface IMySocketService
    {
        Task<Reply> SendNotificacao(ComNotificarRequest comando);
    }
}
