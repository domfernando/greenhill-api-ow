namespace Unit.Application.DTOs.Request
{
    public class QueryNVMCRequest
    {
        public int? Id { get; set; }
        public string? Mes { get; set; }
    }

    public class  CreateNVMCRequest
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }

    public class UpdateNVMCRequest : CreateNVMCRequest
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public string? Tipo { get; set; }
    }    
}
