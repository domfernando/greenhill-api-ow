using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Domain.Entities.Config
{
    public class Servico: EntidadeBase
    {
        public string Nome { get; set; }
        public string Valor { get; set; }
        public bool Ativo { get; set; }
    }
}
