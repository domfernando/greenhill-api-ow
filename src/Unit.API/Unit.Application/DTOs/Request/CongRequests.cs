namespace Unit.Application.DTOs.Request
{
    public class CongQueryModel
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Circuito { get; set; }
        public string? Resposavel { get; set; }
    }
    public class CongNewModel
    {
        public string? Nome { get; set; }
    }
    public class CongUpdateModel
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string? Circuito { get; set; }
        public string? Endereco { get; set; }
        public string? Dia { get; set; }
        public string? Horario { get; set; }
        public string? Responsavel { get; set; }
        public string? Fone { get; set; }
        public string? Email { get; set; }
        public string? Maps { get; set; }
    }
}
