using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Domain.Enums;

namespace Unit.Application.DTOs.Request
{
    public class QueryMensagemRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public int? MessageMode { get; set; }    
    }

    public class CreateMensagemRequest
    {
        public string Nome { get; set; }
        public int MessageMode { get; set; }
    }

    public class UpdateMensagemRequest : CreateMensagemRequest
    {
        public int Id { get; set; }
        public string Conteudo { get; set; }
    }
}
