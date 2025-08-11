namespace Unit.Application.DTOs.Response
{
    public class PerfilResponse
    {
        public int ID { get; set; }
        public string Nome { get; set; }
    }

    public class UsuarioPerfilResponse
    {
        public int ID { get; set; }
        public string? PerfilNome { get; set; }
        public string? UsuarioNome { get; set; }
    }
}
