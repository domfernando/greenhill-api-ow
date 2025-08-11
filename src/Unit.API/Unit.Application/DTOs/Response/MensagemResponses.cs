namespace Unit.Application.DTOs.Response
{
    public class MensagemResponse: BaseResponse
    {
        public string Nome { get; set; }
        public int MessageMode { get; set; }
        public string Conteudo { get; set; }
        public string? ConteudoRenderizado { get; set; }
        public DateTime? Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
