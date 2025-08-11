namespace Unit.Application.DTOs.Response
{
    public class EnderecoResponse: BaseResponse
    {
        public int TipoEndereco { get; set; } // 1 - Residencial, 2 - Comercial, 3 - Entrega, 4 - Cobrança
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
    }
}
