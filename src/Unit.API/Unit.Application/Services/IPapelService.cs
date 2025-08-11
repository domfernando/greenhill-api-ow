using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IPapelService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryPapelRequest condicao);
        Task<Reply> Add(CreatePapelRequest entidade);
        Task<Reply> Update(UpdatePapelRequest entidade);
    }
}
