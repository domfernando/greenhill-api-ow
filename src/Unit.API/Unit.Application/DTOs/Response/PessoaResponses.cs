namespace Unit.Application.DTOs.Response
{
    public class PessoaResponse : BaseResponse
    {
        public string Nome { get; set; }
        public string? NomeCompleto { get; set; }
        public bool? Fisica { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Documento { get; set; }
        public int SituacaoComercial { get; set; }
        public DateTime? Nascimento { get; set; }
        public string? Observacao { get; set; }
        public string? Whatsapp { get; set; }
        public int? Idade { get; set; }
        public List<PessoaPapelResponse> Papeis { get; set; } = new List<PessoaPapelResponse>();
        public List<PessoaEnderecoResponse> Enderecos { get; set; } = new List<PessoaEnderecoResponse>();
    }

    public class PessoaPapelResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class PessoaEnderecoResponse
    {
        public int Id { get; set; }
        public string Sexo { get; set; }
        public int EnderecoId { get; set; }
        public int TipoEndereco { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public bool Principal { get; set; }

    }
}
