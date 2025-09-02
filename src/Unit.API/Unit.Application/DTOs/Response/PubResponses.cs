namespace Unit.Application.DTOs.Response
{
    public class PubResponse
    {
        public int ID { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Nome { get; set; }
        public string? Situacao { get; set; }
        public string? Privilegio { get; set; }
        public bool? Orador { get; set; } = false;
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Whatsapp { get; set; }
        public DateTime? Nascimento { get; set; }
        public string? NascimentoFormatado { get; set; }
        public DateTime? Batismo { get; set; }
        public string? BatismoFormatado { get; set; }
        public string? Genero { get; set; }
        public string? Obs { get; set; }
        public string? EmergenciaNome { get; set; }
        public string? EmergenciaContato { get; set; }
        public bool? Escola { get; set; } = false;
        public bool? Notificar { get; set; } = false;
        public int? UsuarioId { get; set; }
        public PubUsuarioResponse Usuario { get; set; }
        public int? Idade { get; set; }
        public int? TempoBatismo { get; set; } = 0;
        public string? Faixa { get; set; }
        public string? CriadoFormatado { get; set; }
        public string? AlteradoFormatado { get; set; }
        public List<PubPapelResponse>? Papeis { get; set; }
        public List<GrupoPubResponse>? Grupos { get; set; }
    }

    public class PubUsuarioResponse
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Usr { get; set; }
        public List<string> Perfis { get; set; }
    }

    public class PubPapelResponse
    {
        public int ID { get; set; }
        public int PapelID { get; set; }
        public string? NomePublicador { get; set; }
        public string? NomePapel { get; set; }
    }

    public class GrupoPubResponse
    {
        public int ID { get; set; }
        public int GrupoID { get; set; }
        public string? NomeGrupo { get; set; }
        public string? Papel { get; set; }
    }
}
