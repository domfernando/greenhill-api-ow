namespace Unit.Application.DTOs.Request
{
    public class QueryPerfilRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }

    public class  CreatePerfilRequest
    {
        public string Nome { get; set; }
    }

    public class UpdatePerfilRequest : CreatePerfilRequest
    {
        public int Id { get; set; }
    }

    public class RemovePerfilUsuarioRequest
    {
        public int ID { get; set; }
    }

    public class AdicionaPerfilUsuarioRequest
    {
        public int PerfilId { get; set; }
        public int UsuarioId { get; set; }
    }
}
