using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IServicoService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryServicoRequest condicao);
        Task<Reply> Add(CreateServicoRequest entidade);
        Task<Reply> Update(UpdateServicoRequest entidade);
        Task<ConfigServico> ConfigServico(string nome);
    }
}
