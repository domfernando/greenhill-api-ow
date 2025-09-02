namespace Unit.Application.DTOs.Response
{
    public class PapelResponse: BaseResponse
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public List<PubPapelResponse> Pubs { get; set; }
    }
}
