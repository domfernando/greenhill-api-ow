namespace Unit.Application.DTOs.Request
{
    public class QueryPessoaRequest
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }

    public class CreatePessoaRequest
    {
        public string Nome { get; set; }
        public bool Fisica { get; set; }
    }

    public class UpdatePessoaRequest : CreatePessoaRequest
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string? Sexo { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Documento { get; set; }
        public int SituacaoComercial { get; set; }
        public DateTime? Nascimento { get; set; }
        public string? Observacao { get; set; }
    }

    public class AddPapelPessoaRequest
    {
        public int PessoaId { get; set; }
        public int PapelId { get; set; }
    }
}
