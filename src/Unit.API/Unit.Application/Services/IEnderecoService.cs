using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IEnderecoService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryEnderecoRequest condicao);
        Task<Reply> Add(CreateEnderecoRequest entidade, int pessoaId);
        Task<Reply> Update(UpdateEnderecoRequest entidade);
    }
}
