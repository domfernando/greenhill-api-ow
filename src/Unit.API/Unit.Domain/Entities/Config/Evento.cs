using System.ComponentModel.DataAnnotations.Schema;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Domain.Entities.Config
{
    public class Evento : EntidadeBase
    {
        public DateTime Data { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public bool Semana { get; set; }
        public string? Tipo { get; set; }
        public virtual List<EventoPapel> Papeis { get; set; } = new List<EventoPapel>();

        [NotMapped]
        public string Mes
        {
            get
            {
                return Data.ToString("MMMM");
            }
        }

        [NotMapped]
        public string DataFormatada
        {
            get
            {
                if (Data != null && Data != DateTime.MinValue)
                {
                    return Data.ToString("dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }     
    }

    public class EventoPapel : EntidadeBase
    {
        public int EventoId { get; set; }
        public int PapelId { get; set; }
        public virtual Evento Evento { get; set; }
        public virtual Papel Papel { get; set; }
    }
}
