using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Domain.Entities.Cadastro
{
    public class Papel: EntidadeBase
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }
}
