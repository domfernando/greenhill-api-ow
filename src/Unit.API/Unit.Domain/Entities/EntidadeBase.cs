using System;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Unit.Domain
{
    public class EntidadeBase
    {
        public int ID { get; set; }

        public System.DateTime? Criado { get; set; }

        public System.DateTime? Alterado { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || (!obj.GetType().Equals(this.GetType())))
                return false;
            if (this == obj)
                return true;

            return (this.ID == ((EntidadeBase)obj).ID);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}#{1}", this.ID.ToString(), this.GetType());
        }

        [NotMapped]
        public string? CriadoFormatado
        {
            get
            {
                if (Criado != null && Criado != DateTime.MinValue)
                {
                    return string.Format("{0:dd/MM/yyyy HH:mm:ss}", Criado);
                }
                else
                {
                    return "";
                }
            }
        }
        [NotMapped]
        public string? AlteradoFormatado
        {
            get
            {
                if (Alterado != null && Alterado != DateTime.MinValue)
                {
                    return string.Format("{0:dd/MM/yyyy HH:mm:ss}", Alterado);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
