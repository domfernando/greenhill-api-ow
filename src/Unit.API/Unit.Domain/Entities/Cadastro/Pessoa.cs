using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Unit.Domain.Entities.Cadastro
{
    public class Pessoa: EntidadeBase
    {
        public string Nome { get; set; }
        public string? NomeCompleto { get; set; }
        public bool? Fisica { get; set; }
        public string? Sexo { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Documento { get; set; }
        public int SituacaoComercial { get; set; }
        public DateTime? Nascimento { get; set; }
        public string? Observacao { get; set; }

        public virtual List<PessoaPapel> Papeis { get; set; }
        public virtual List<PessoaEndereco> Enderecos { get; set; }

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
        public string Whatsapp
        {
            get
            {
                if (!string.IsNullOrEmpty(Celular))
                {
                    string _whatsapp = Regex.Replace(Celular, @"[^\d]", "");

                    _whatsapp = _whatsapp.Substring(0, 2) == "55" ? _whatsapp
                                                                  : $"55{_whatsapp}";
                    return _whatsapp;

                }
                return "";
            }
        }
    }

    public class PessoaPapel : EntidadeBase
    {
        public int PessoaId { get; set; }
        public int PapelId { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Papel Papel { get; set; }
    }

    public class PessoaEndereco : EntidadeBase
    {
        public int PessoaId { get; set; }
        public int EnderecoId { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Endereco Endereco { get; set; }
    }
}
