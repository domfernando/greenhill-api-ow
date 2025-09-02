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
        public string? Descricao { get; set; }
        public string? Tipo { get; set; }
    }

    public class RemovePubPapelRequest
    {
        public int Id { get; set; }
    }

    public class AdicionaPubPapelRequest
    {
        public int PubId { get; set; }
        public int PapelId { get; set; }
    }
}
