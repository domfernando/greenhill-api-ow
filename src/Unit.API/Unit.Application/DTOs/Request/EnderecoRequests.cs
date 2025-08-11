namespace Unit.Application.DTOs.Request
{
    public class QueryEnderecoRequest
    {
        public int? Id { get; set; }
        public string? Logradouro { get; set; }
    }

    public class  CreateEnderecoRequest
    {
        public int PessoaId { get; set; }
        public int TipoEndereco { get; set; } // 1 - Residencial, 2 - Comercial, 3 - Entrega, 4 - Cobrança
        public string Logradouro { get; set; }
        public string Numero { get; set; }
    }

    public class UpdateEnderecoRequest : CreateEnderecoRequest
    {
        public int Id { get; set; }
        public int TipoEndereco { get; set; } // 1 - Residencial, 2 - Comercial, 3 - Entrega, 4 - Cobrança
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
    }
}
