namespace Unit.Application.DTOs.Request
{
    public class QueryRelatorioRequest
    {
        public int? Id { get; set; }
        public DateTime? Mes { get; set; }
        public int? PubId { get; set; }
        public int? GrupoId { get; set; }
    }

    public class CreateRelatorioRequest
    {
        public DateTime Mes { get; set; }
        public int PubId { get; set; }
    }

    public class CreateRelatorioBatchRequest
    {
        public DateTime Mes { get; set; }
    }

    public class CreateRelatorioByGrupoRequest
    {
        public DateTime Data { get; set; }
        public int GrupoId { get; set; }
    }

    public class UpdateRelatorioRequest : CreateRelatorioRequest
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int PubId { get; set; }
        public bool? Atividade { get; set; } = false;
        public bool? Auxiliar { get; set; } = false;
        public int? Horas { get; set; } = 0;
        public int? Estudos { get; set; } = 0;
        public string? Obs { get; set; }
        public bool? Entregue { get; set; } = false;
    }
}
