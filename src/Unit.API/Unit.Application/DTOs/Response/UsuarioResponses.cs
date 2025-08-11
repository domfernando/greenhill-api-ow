using Unit.Domain.Entities.Acesso;

namespace Unit.Application.DTOs.Response
{
    public class UsuarioResponse: BaseResponse
    {
        public string Nome { get; set; }
        public string NomeCompleto { get; set; }
        public string Usr { get; set; }
        public string Email { get; set; }
        public string UltimoLogin { get; set; }
        public List<UsuarioPerfilResponse> Perfis { get; set; }
    }

    public class UsuarioLoginResponse
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string NomeCompleto { get; set; }
        public string Usr { get; set; }
        public string Email { get; set; }
        public string UltimoLogin { get; set; }
        public List<UsuarioPerfilResponse> Perfis { get; set; }
    }

    public class UsuarioCRUDResponse: BaseResponse
    {
        public string? Nome { get; set; }
        public string? NomeCompleto { get; set; }
        public string Email { get; set; }
        public int? DDI { get; set; }
        public string? Celular { get; set; }
        public string? Whatsapp { get; set; }
        public string? Usr { get; set; }
        public string? Codigo { get; set; }
        public bool? Mfa { get; set; }
        public string? MfaModo { get; set; }
        public string? MfaCodigo { get; set; }
        public DateTime? MfaExpira { get; set; }
        public bool? Ativo { get; set; }
        public int? Tentativa { get; set; }
        public string? Travado { get; set; }
        public string? UltimoLogin { get; set; }
        public List<UsuarioPerfilResponse?> Perfis { get; set; }
    }
}
