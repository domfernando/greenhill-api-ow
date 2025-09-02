using Unit.Domain;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.DTOs.Response
{
    public class ArranjoResponse
    {
        public int ID { get; set; }
        public int CongId { get; set; }
        public CongResponse Cong { get; set; }
        public string? Mes{ get; set; }
        public string? Modo { get; set; }
        public string? Status { get; set; }
        public string? Obs { get; set; }
        public List<DiscursoResponse> Discursos { get; set; }
        public List<ArranjoNotificacaoResponse> Notificacoes { get; set; }=new List<ArranjoNotificacaoResponse>();
        public string? Criado { get; set; }
        public string? Alterado { get; set; }
    }

    public class ArranjoNotificacaoResponse
    {
        public int Id { get; set; }
        public int ArranjoID { get; set; }
        public virtual ArranjoResponse Arranjo { get; set; }
        public int QueueID { get; set; }
        public string Tipo { get; set; }
        //public virtual Queue Queue { get; set; }
    }

    public class TemaResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string? NomeFormatado { get; set; }
    }

    public class OradorResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Privilegio { get; set; }
        public string Celular { get; set; }
        public List<OradorTemasResponse> Temas { get; set; }
    }

    public class OradorTemasResponse
    {
        public int Id { get; set; }
        public TemaResponse Tema { get; set; }
    }

    public class DiscursoResponse
    {
        public int Id { get; set; }
        public int ArranjoId { get; set; }
        public ArranjoResponse Arranjo { get; set; }
        public int TemaId { get; set; }
        public TemaResponse Tema { get; set; }
        public string? Data { get; set; }
        public string? DataFormatada { get; set; }
        public string? Mes { get; set; }
        public string Orador { get; set; }
        public string Status { get; set; }
        public bool ? Recebido { get; set; }=false;
        //public List<NotificacaoResponse> Notificacoes { get; set; } = new List<NotificacaoResponse>();
    }
}
