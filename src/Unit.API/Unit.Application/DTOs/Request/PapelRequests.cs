namespace Unit.Application.DTOs.Request
{
    public class QueryPapelRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }

    public class  CreatePapelRequest
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }

    public class UpdatePapelRequest : CreatePapelRequest
    {
        public int Id { get; set; }
    }
}
