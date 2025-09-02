using Unit.Domain.Entities.Config;

namespace Unit.Domain.Entities.Cadastro
{
    public class Papel: EntidadeBase
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public virtual List<PubPapel> Pubs { get; set; }
        public virtual List<EventoPapel> Eventos { get; set; } = new List<EventoPapel>();
    }
}
