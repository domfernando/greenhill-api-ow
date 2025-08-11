namespace Unit.Domain.Entities.Acesso
{
    using Unit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.RegularExpressions;

    public class Usuario : EntidadeBase
    {
        public string? Nome { get; set; }
        public string? NomeCompleto { get; set; }
        public string Email { get; set; }
        public string? Celular { get; set; }
        public string? Usr { get; set; }
        public string? Pwd { get; set; }
        public string? Codigo { get; set; }
        public bool? Mfa { get; set; }
        public string? MfaModo { get; set; }
        public string? MfaCodigo { get; set; }
        public DateTime? MfaExpira { get; set; }
        public bool? Ativo { get; set; }
        public int? Tentativa { get; set; }
        public string? Travado { get; set; }
        public string? Verificado { get; set; }
        public string? UltimoLogin { get; set; }
        public List<UsuarioPerfil?> Perfis { get; set; }

        [NotMapped]
        public string Whatsapp
        {
            get
            {
                if (!string.IsNullOrEmpty(Celular))
                {
                    string _whatsapp = Regex.Replace(Celular, @"[^\d]", "");

                    _whatsapp = _whatsapp.Substring(0, 2) == "55" ? _whatsapp
                                                                  : $"55{_whatsapp}";
                    return _whatsapp;
                }
                return "";
            }
        }
    }

    public class Perfil : EntidadeBase
    {
        [DataType("varchar(50)")]
        public string Nome { get; set; }
        public virtual List<UsuarioPerfil> Usuarios { get; set; }
    }

    public class UsuarioPerfil : EntidadeBase
    {
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public int PerfilId { get; set; }
        public virtual Perfil Perfil { get; set; }
    }
}
