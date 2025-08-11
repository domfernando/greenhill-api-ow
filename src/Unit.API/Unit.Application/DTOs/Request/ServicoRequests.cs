namespace Unit.Application.DTOs.Request
{
    public class QueryServicoRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }

    public class  CreateServicoRequest
    {
        public string Nome { get; set; }
    }

    public class UpdateServicoRequest : CreateServicoRequest
    {
        public int Id { get; set; }
        public string Valor { get; set; }
    }
}
