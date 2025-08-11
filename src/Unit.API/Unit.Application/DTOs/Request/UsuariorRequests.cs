namespace Unit.Application.DTOs.Request
{
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Pwd { get; set; }
    }

    public class GetMfaTokenRequest
    {
        public string Email { get; set; }
    }

    public class LoginMfaRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
    public class QueryUsuarioRequest
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
    }

    public class  CreateUsuarioRequest
    {
        public string Email { get; set; }
        public string Nome { get; set; }
    }

    public class UpdateUsuarioRequest : CreateUsuarioRequest
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string? Pwd { get; set; }
        public string? Travado { get; set; }
        public bool? Mfa { get; set; }
        public string? MfaModo { get; set; }
        public string? MfaCodigo { get; set; }
        public DateTime? MfaExpira { get; set; }
        public string? Verificado { get; set; }
        public string? Celular { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
