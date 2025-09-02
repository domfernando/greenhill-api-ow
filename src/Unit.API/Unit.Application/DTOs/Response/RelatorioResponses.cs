namespace Unit.Application.DTOs.Response
{
    public class RelatorioResponse : BaseResponse
    {
        public DateTime Data { get; set; }
        public int PubId { get; set; }
        public string Nome { get; set; }
        public string? Privilegio { get; set; }
        public string? Situacao { get; set; }
        public string? Obs { get; set; }
        public bool Entregue { get; set; }
        public bool Atividade { get; set; }
        public bool Auxiliar { get; set; }
        public bool Regular { get; set; }
        public int Horas { get; set; }
        public int Estudos { get; set; }
        public bool Ativo { get; set; }
        public string Mes { get; set; }
    }
}
