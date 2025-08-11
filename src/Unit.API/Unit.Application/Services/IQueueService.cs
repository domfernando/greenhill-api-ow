using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.DTOs.Request;
using Unit.Application.Util;

namespace Unit.Application.Sevices
{
    public interface IQueueService
    {
        Task<Reply> GetOne(int id);
        Task<Reply> GetAll(QueryQueueRequest condicao);
        Task<Reply> GetForProcess(QueryQueueForProcess condicao);
        Task<Reply> ProcessQueue(QueryQueueForProcess condicao);
        Task<Reply> Add(CreateQueueRequest entidade);
        Task<Reply> Update(UpdateQueueRequest entidade);
        Task<Reply> GetGraphics(QueryQueueGraphicsRequest condicao);
    }
}
