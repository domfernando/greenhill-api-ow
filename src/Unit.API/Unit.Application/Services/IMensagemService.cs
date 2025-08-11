using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;
using Unit.Domain.Entities.Config;

namespace Unit.Application.Sevices
{
    public interface IMensagemService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryMensagemRequest condicao);
        Task<Reply> Add(CreateMensagemRequest entidade);
        Task<Reply> Update(UpdateMensagemRequest entidade);
        Task<Reply> Render(Mensagem msg);
    }
}
