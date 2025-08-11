using System;

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
    }
}
