using System.ComponentModel.DataAnnotations.Schema;

namespace Unit.Domain.Entities.Config
{
    public class Mensagem : EntidadeBase
    {
        public string Nome { get; set; }
        public string? Conteudo { get; set; }
        public int MessageMode { get; set; }

        [NotMapped]
        public string ConteudoRenderizado
        {
            get
            {
                string renderizado = Conteudo;

                if (MessageMode == 1 && !string.IsNullOrEmpty(Conteudo))
                {
                    renderizado = Conteudo.Replace("<b>", "*").Replace("</b>", "*")
                                        .Replace("<h1>", "*").Replace("</h1>", "*")
                                        .Replace("<p>", "").Replace("</p>", "\n").Replace("<p></p>", "\n")
                                        .Replace("<strong>", "*").Replace("</strong>", "*").Replace("<strong></strong>", "")
                                        .Replace("<i>", "_").Replace("</i>", "_")
                                        .Replace("<em>", "_").Replace("</em>", "_")
                                        .Replace("<del>", "~").Replace("</del>", "~")
                                        .Replace("<code>", "```").Replace("</code>", "```")
                                        .Replace("<br>", "\n").Replace("<br />", "")
                                        .Replace("&nbsp;", " ");
                }

                return renderizado;
            }
        }

        [NotMapped]
        public List<MensagemReplace> Marcadores { get; set; } = new List<MensagemReplace>();
    }

    public class MensagemReplace
    {
        public string Marcador { get; set; }
        public string Valor { get; set; }
    }
}