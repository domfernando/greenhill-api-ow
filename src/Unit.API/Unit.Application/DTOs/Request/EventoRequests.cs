namespace Unit.Application.DTOs.Request
{
    public class QueryEventoRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Mes { get; set; }
        public string? Ano { get; set; }
    }

    public class QueryEventoByPapelRequest
    {
        public string? IdsSelecionados { get; set; }
        public bool? Todos { get; set; } = false;
        public int Quantidade { get; set; } = 3;
    }

    public class  CreateEventoRequest
    {
        public string Nome { get; set; }
        public DateTime Data { get; set; }
    }

    public class UpdateEventoRequest : CreateEventoRequest
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public bool Semana { get; set; } = false;
        public string? Tipo { get; set; }
        public string? PapeisSelecionados { get; set; }

    }
}
