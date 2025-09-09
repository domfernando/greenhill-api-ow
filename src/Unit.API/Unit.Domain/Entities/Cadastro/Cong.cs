using System.ComponentModel.DataAnnotations.Schema;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Extra;

namespace Unit.Domain.Entities.Cadastro
{
    public class Pub : EntidadeBase
    {
        public string NomeCompleto { get; set; }
        public string Nome { get; set; }
        public string? Situacao { get; set; }
        public string? Privilegio { get; set; }
        public bool? Orador { get; set; } = false;
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public DateTime? Nascimento { get; set; }
        public DateTime? Batismo { get; set; }
        public DateTime? AuxiliarAte { get; set; }
        public string? Genero { get; set; }
        public string? Obs { get; set; }
        public string? EmergenciaNome { get; set; }
        public string? EmergenciaContato { get; set; }
        public bool? Escola { get; set; } = false;
        public bool? Notificar { get; set; } = false;
        public int? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual List<OradorTema> Temas { get; set; }
        public virtual List<PubPapel> Papeis { get; set; }  
        public virtual List<GrupoPub> Grupos { get; set; }

        [NotMapped]
        public string? NascimentoFormatado
        {
            get
            {
                if (Nascimento != null && Nascimento != DateTime.MinValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", Nascimento);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public string? BatismoFormatado
        {
            get
            {
                if (Batismo != null && Batismo != DateTime.MinValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", Nascimento);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public int? Idade
        {
            get
            {
                if (Nascimento != null)
                {
                    var dateSpan = DateTime.Compare(Nascimento.Value, DateTime.Now);
                    int idade = DateTime.Now.Year - Nascimento.Value.Year;
                    if (DateTime.Now.DayOfYear < Nascimento.Value.DayOfYear)
                    {
                        idade = idade - 1;
                    }
                    return idade;
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public string? IdadeFormatada
        {
            get
            {
                if (Nascimento != null)
                {
                    var hoje = DateTime.Now;
                    var anos = hoje.Year - Nascimento.Value.Year;
                    var meses = hoje.Month - Nascimento.Value.Month;

                    if(meses < 0)
                    {
                        anos--;
                        meses += 12;
                    }
                    
                    return $"{anos} anos e {meses} meses";
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public int? TempoBatismo
        {
            get
            {
                if (Batismo != null)
                {
                    int tempo = DateTime.Now.Year - Batismo.Value.Year;
                    return tempo;
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public string? TempoBatismoFormatado
        {
            get
            {
                if (Batismo != null)
                {
                    var hoje = DateTime.Now;
                    var anos = hoje.Year -  Batismo.Value.Year;
                    var meses = hoje.Month - Batismo.Value.Month;

                    if (meses < 0)
                    {
                        anos--;
                        meses += 12;
                    }

                    return $"{anos} anos e {meses} meses";
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public bool Dianteira
        {
            get
            {
                return Genero == "masculino" &&
                       (Privilegio == "Ancião" || Privilegio == "Servo Ministerial");
            }
        }

        [NotMapped]
        public bool Varao
        {
            get
            {
                return Genero == "masculino" &&
                    (Privilegio == "Ancião" || Privilegio == "Servo Ministerial" ||
                     Privilegio == "Publicador Exemplar");
            }
        }

        [NotMapped]
        public string Whatsapp
        {
            get
            {
                if (!string.IsNullOrEmpty(Celular))
                {
                    string _whatsapp = Celular.Replace("(", "").Replace(")", "")
                                              .Replace("-", "").Replace(" ", "");
                    _whatsapp = _whatsapp.Substring(0, 2) == "55" ? _whatsapp
                                                                 : $"55{_whatsapp}";
                    return _whatsapp;
                }
                return "";
            }
        }
        [NotMapped]
        public string Faixa
        {
            get
            {
                switch (Idade)
                {
                    case null:
                    case 0:
                        return "Indefinido";
                    case >= 1 and <= 12:
                        return "Criança";
                    case >= 13 and <= 18:
                        return "Adolescente";
                    case >= 19 and <= 29:
                        return "Jovem";
                    case >= 30 and <= 59:
                        return "Adulto";
                    case >= 60:
                        return "Idoso";
                    default:
                        return "Indefinido";
                }
            }
        }
    }
    public class Cong : EntidadeBase
    {
        public string Nome { get; set; }
        public string? Circuito { get; set; }
        public string? Endereco { get; set; }
        public string? Dia { get; set; }
        public string? Horario { get; set; }
        public string? Responsavel { get; set; }
        public string? Fone { get; set; }
        public string? Email { get; set; }
        public string? Maps { get; set; }
        public virtual List<Arranjo>? Arranjos { get; set; }
        [NotMapped]
        public string Whatsapp
        {
            get
            {
                if (!string.IsNullOrEmpty(Fone))
                {
                    string _whatsapp = Fone.Replace("(", "").Replace(")", "")
                                           .Replace("-", "").Replace(" ", "");
                    _whatsapp = _whatsapp.Substring(0, 2) == "55" ? _whatsapp
                                                                 : $"55{_whatsapp}";
                    return _whatsapp;
                }
                return "";
            }
        }
    }
    public class Grupo : EntidadeBase
    {
        public string Nome { get; set; }
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
        public virtual List<GrupoPub> Membros { get; set; }
    }
    public class GrupoPub : EntidadeBase
    {
        public int GrupoID { get; set; }
        public virtual Grupo Grupo { get; set; }
        public int PubID { get; set; }
        public virtual Pub Pub { get; set; }
        public string Papel { get; set; }
    }
    public class Tema : EntidadeBase
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        [NotMapped]
        public List<Discurso?> Discursos { get; set; }
        [NotMapped]
        public List<OradorTema?> Oradores { get; set; }
    }
    public class OradorTema : EntidadeBase
    {
        public int PubID { get; set; }
        public virtual Pub Orador { get; set; }
        public int TemaID { get; set; }
        public virtual Tema Tema { get; set; }
        public bool Publico { get; set; }
    }
    public class Arranjo : EntidadeBase
    {
        public int CongId { get; set; }
        public virtual Cong Cong { get; set; }
        public DateTime? Data { get; set; }
        public string? Modo { get; set; }
        public string? Status { get; set; }
        public string? Obs { get; set; }
        public virtual List<Discurso>? Discursos { get; set; }
        public virtual List<ArranjoNotificacao>? Notificacoes { get; set; }

        [NotMapped]
        public string Mes
        {
            get
            {
                if (Data != null)
                {
                    return string.Format("{0:MMMM/yyyy}", Data);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public string Ano
        {
            get
            {
                if (Data != null)
                {
                    return string.Format("{0:yyyy}", Data);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public string NumeroMes
        {
            get
            {
                if (Data != null)
                {
                    return Data.Value.Month.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
    }
    public class Discurso : EntidadeBase
    {
        public int ArranjoID { get; set; }
        public virtual Arranjo Arranjo { get; set; }
        public DateTime? Data { get; set; }
        public int TemaID { get; set; }
        public virtual Tema Tema { get; set; }
        public string? Orador { get; set; }
        public string? Contato { get; set; }
        public string? Status { get; set; }
        public string? Obs { get; set; }
        public bool? Recebido { get; set; }
        public bool? Local { get; set; }
        public virtual List<DiscursoNotificacao>? Notificacoes { get; set; }

        [NotMapped]
        public string DataFormatada
        {
            get
            {
                if (Data != null)
                {
                    return string.Format("{0:dd/MM/yyyy}", Data);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public string Mes
        {
            get
            {
                if (Data != null)
                {
                    return string.Format(new System.Globalization.CultureInfo("pt-BR"), "{0:MMMM/yyyy}", Data);
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public string Whatsapp
        {
            get
            {
                if (!string.IsNullOrEmpty(Contato))
                {
                    string _whatsapp = Contato.Replace("(", "").Replace(")", "")
                                              .Replace("-", "").Replace(" ", "");
                    _whatsapp = _whatsapp.Substring(0, 2) == "55" ? _whatsapp
                                                                 : $"55{_whatsapp}";
                    return _whatsapp;
                }
                return "";
            }
        }
    }
}
