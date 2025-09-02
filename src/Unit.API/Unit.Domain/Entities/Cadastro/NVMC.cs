namespace Unit.Domain.Entities.Cadastro
{
    public class NVMC: EntidadeBase
    {
        public DateTime Mes { get; set; }
        public DateTime Data { get; set; }
        public string Presidente { get; set; } = string.Empty;
        public string OracaoInicial { get; set; } = string.Empty;
        public string OracaoFinal { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public virtual ICollection<NVMCParte> Partes { get; set; } = new List<NVMCParte>();
    }

    public class NVMCParte: EntidadeBase
    {
        public int NVMCID { get; set; }
        public virtual NVMC NVMC { get; set; } = null!;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int? DesignadoID { get; set; }
        public virtual Pub Designado { get; set; } = null!;
        public int? AjudanteID { get; set; }
        public virtual Pub? Ajudante { get; set; } = null!;
        public string NomeSecao { get; set; } = string.Empty;
        public string Privilegio { get; set; } = string.Empty;

    }
}
