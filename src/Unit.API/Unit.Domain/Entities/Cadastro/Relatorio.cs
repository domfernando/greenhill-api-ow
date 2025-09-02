using System.ComponentModel.DataAnnotations.Schema;

namespace Unit.Domain.Entities.Cadastro
{
    public class Relatorio : EntidadeBase
    {
        public DateTime Data { get; set; }
        public int PubId { get; set; }
        public bool? Atividade { get; set; } = false;
        public bool? Auxiliar { get; set; } = false;
        public bool? Regular { get; set; } = false;
        public int? Horas { get; set; } = 0;
        public int? Estudos { get; set; } = 0;
        public bool? Entregue { get; set; } = false;
        public string? Obs { get; set; }
        public virtual Pub Pub { get; set; }

        [NotMapped]
        public string Mes
        {
            get
            {
                if (Data != null)
                {
                    return string.Format("{0:MMMM/yyyy}", Data);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
