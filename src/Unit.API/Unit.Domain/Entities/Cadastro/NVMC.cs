using System.ComponentModel.DataAnnotations.Schema;

namespace Unit.Domain.Entities.Cadastro
{
    public class NVMC : EntidadeBase
    {
        public DateTime Mes { get; set; }
        public DateTime Data { get; set; }
        public string? Semana { get; set; }
        public string Presidente { get; set; } = string.Empty;
        public string OracaoInicial { get; set; } = string.Empty;
        public string OracaoFinal { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public virtual ICollection<NVMCParte> Partes { get; set; } = new List<NVMCParte>();

        [NotMapped]
        public string MesFormatado
        {
            get
            {
                if (Mes != null && Mes != DateTime.MinValue)
                {
                    return string.Format("{0:MMMM/yyyy}", Mes);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public string DataFormatada
        {
            get
            {
                if (Data != null && Data != DateTime.MinValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", Data);
                }
                else
                {
                    return "";
                }
            }
        }
    }

    public class NVMCParte : EntidadeBase
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

        [NotMapped]
        public string DescricaoParte
        {
            get
            {
                string descricao = string.Empty;

                if (Titulo.ToLower().Contains("1. "))
                {
                    descricao = "Tesouros";
                }
                else if (Titulo.ToLower().Contains("2. "))
                {
                    descricao = "Jóias";
                }
                else if (Titulo.ToLower().Contains("3. "))
                {
                    descricao = "Leitura";
                }
                else
                {
                    if (NomeSecao.ToLower().Contains("faça"))
                    {
                        if (Titulo.ToLower().Contains("discurso"))
                        {
                            descricao = "Discurso";
                        }
                        else
                        {
                            descricao = $"Estudante";
                        }
                    }
                    else if (NomeSecao.ToLower().Contains("nossa"))
                    {
                        if (Titulo.ToLower().Contains("estudo bíblico de congregação"))
                        {
                            descricao = "Estudo de Livro";
                        }
                        else if (Titulo.ToLower().Contains("necessidades locais"))
                        {
                            descricao = "Necessidades Locais";
                        }
                        else
                        {
                            descricao = "Consideração";
                        }
                    }
                    else
                    {
                        descricao = Titulo;
                    }
                }

                return descricao;
            }
        }

        [NotMapped]
        public string DescricaoDesignado
        {
            get
            {
                string descricao = string.Empty;

                if (Titulo.ToLower().Contains("1. "))
                {
                    descricao = "Tesouros";
                }
                else if (Titulo.ToLower().Contains("2. "))
                {
                    descricao = "Jóias";
                }
                else if (Titulo.ToLower().Contains("3. "))
                {
                    descricao = "Leitura";
                }
                else
                {
                    if (NomeSecao.ToLower().Contains("faça"))
                    {
                        if (Titulo.ToLower().Contains("discurso"))
                        {
                            descricao = "Discurso";
                        }
                        else
                        {
                            descricao = $"{Titulo}(Designado)";
                        }
                    }
                    else if (NomeSecao.ToLower().Contains("nossa"))
                    {
                        if (Titulo.ToLower().Contains("estudo bíblico de congregação"))
                        {
                            descricao = $"{Titulo}(Dirigente)";
                        }
                        else
                        {
                            descricao = $"{Titulo}(Designado)";
                        }
                    }
                    else
                    {
                        descricao = Titulo;
                    }
                }

                return descricao;
            }
        }

        [NotMapped]
        public string DescricaoAjudante
        {
            get
            {
                string descricao = string.Empty;

                if (NomeSecao.ToLower().Contains("faça"))
                {
                    descricao = $"{Titulo}(Ajudante)";
                }
                else if (NomeSecao.ToLower().Contains("nossa"))
                {
                    if (Titulo.ToLower().Contains("estudo bíblico de congregação"))
                    {
                        descricao = $"Leitor";
                    }
                    else
                    {
                        descricao = $"{Titulo}(Designado)";
                    }
                }
                else
                {
                    descricao = Titulo;
                }

                return descricao;
            }
        }
    }
}
