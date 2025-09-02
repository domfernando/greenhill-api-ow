namespace Unit.Application.DTOs.Request
{
    public class QueryGrupoRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }

    public class  CreateGrupoRequest
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }

    public class UpdateGrupoRequest : CreateGrupoRequest
    {
        public int Id { get; set; }
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
    }

    public class RemoveGrupoPubRequest
    {
        public int Id { get; set; }
    }

    public class AdicionaGrupoPubRequest
    {
        public int PubId { get; set; }
        public int GrupoId { get; set; }
        public string? Papel { get; set; }
    }

    public class UpdateGrupoPubRequest
    {
        public int Id { get; set; }
        public int GrupoId { get; set; }
        public string Papel { get; set; }
    }
}
