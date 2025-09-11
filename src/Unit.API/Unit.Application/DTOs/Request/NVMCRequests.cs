namespace Unit.Application.DTOs.Request
{
    #region NVMC
    public class QueryNVMCRequest
    {
        public int? Id { get; set; }
        public string? Mes { get; set; }
        public string? Data { get; set; }
    }

    public class  CreateNVMCRequest
    {
        public string Mes { get; set; }
        public string Data { get; set; }
        public string Presidente { get; set; }
        public string OracaoInicial { get; set; }
        public string OracaoFinal { get; set; }
        public string? Conteudo { get; set; }
    }

    public class UpdateNVMCRequest : CreateNVMCRequest
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public string? Tipo { get; set; }
    }
    #endregion

    #region NVMCParte

    public class QueryNVMCParteRequest
    {
        public int? Id { get; set; }
        public int? NVMCId { get; set; }
        public string? Data { get; set; }
    }

    public class UpdateNVMCParteRequest
    {
        public int Id { get; set; }
        public int NVMCId { get; set; }
        public string NomeSecao { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Privilegio { get; set; }
        public int? DesignadoId { get; set; } = 0;
        public int? AjudanteId { get; set; } = 0;
    }

    #endregion

    public class NVMCApostilaRequest
    {
        public List<NVMCSemanaRequest> Semanas { get; set; } = new List<NVMCSemanaRequest>();
    }

    public class NVMCSemanaRequest
    {
        public DateTime Data { get; set; } // Data da quinta-feira
        public string Mes { get; set; } = string.Empty;
        public string Presidente { get; set; } = string.Empty;
        public string OracaoInicial { get; set; } = string.Empty;
        public string OracaoFinal { get; set; } = string.Empty;
        public List<NVMCSecaoRequest> Secoes { get; set; } = new List<NVMCSecaoRequest>();
        public string DataFormatada
        {
            get
            {
                if (Data != null && Data != DateTime.MinValue)
                {
                    return Data.ToString("dd/MM/yyyy");
                }
                return string.Empty;
            }
        }
    }
    public class NVMCSecaoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public List<NVMCParteRequest> Partes { get; set; } = new List<NVMCParteRequest>();
    }

    public class NVMCParteRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Designado { get; set; } = string.Empty;
        public string Ajudante { get; set; } = string.Empty;
        public string NomeSecao { get; set; } = string.Empty;
        public string Privilegio
        {
            get
            {
                if (string.IsNullOrEmpty(Titulo))
                {
                    return "";
                }
                string privilegio = string.Empty;

                if (NomeSecao.ToUpper() == "NOSSA VIDA CRISTÃ")
                {
                    privilegio = "DIANTEIRA";
                }
                else
                {
                    switch (Titulo.Split(".")[0])
                    {
                        case "1":
                        case "2":
                            privilegio = "DIANTEIRA";
                            break;
                        case "3":
                            privilegio = "VARAO";
                            break;
                        case "6":
                            privilegio = Titulo.Split(".")[1].ToUpper().Contains("DISCURSO") ? "VARÃO" : "ESTUDANTE";
                            break;
                        default:
                            privilegio = "ESTUDANTE";
                            break;
                    }
                }

                return privilegio;
            }
        }
    }
}
