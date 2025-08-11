using System;
using System.Collections.Generic;
using System.Text;

namespace Unit.Domain.Entities.Util
{
    public class ConfigAccess
    {
        public bool LoginEmail { get; set; }
        public int TamanhoSenha { get; set; }
        public bool BloquearTentativaAcesso { get; set; }
        public bool ValidaEmail { get; set; }
        public int TentativaAcesso { get; set; }
        public int DuracaoToken { get; set; }
        public int DuracaoTokenMfa { get; set; }
        public string EvolutionInstance { get; set; }
    }
}
