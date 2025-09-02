namespace Unit.Domain.Entities.Config
{
    public class Servico: EntidadeBase
    {
        public string Nome { get; set; }
        public string Valor { get; set; }
        public bool Ativo { get; set; }
    }
}
