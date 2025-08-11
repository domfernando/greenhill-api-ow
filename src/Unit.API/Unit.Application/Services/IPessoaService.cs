using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IPessoaService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryPessoaRequest condicao);
        Task<Reply> Add(CreatePessoaRequest entidade);
        Task<Reply> Update(UpdatePessoaRequest entidade);
        Task<Reply> AddPapel(int pessoaId, int papelId);
        Task<Reply> RemPapel(int id);
        Task<Reply> GetGraphics();
    }
}
