using Unit.Application.Enums;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.DTOs.Request
{
    #region Arranjo
    public class ArranjoQueryRequest
    {
        public int? Id { get; set; }
        public int? CongId { get; set; }
        public string? Mes { get; set; }
        public string? Ano { get; set; }
    }
    public class ArranjoNewRequest
    {
        public int CongId { get; set; }
        public string? Data { get; set; }
    }
    public class ArranjoUpdateRequest
    {
        public int ID { get; set; }
        public int CongId { get; set; }
        public string? Data { get; set; }
        public string? Modo { get; set; }
        public string? Status { get; set; }
        public string? Obs { get; set; }
    }
    #endregion

    #region Tema
    public class TemaQueryRequest
    {
        public int? Id { get; set; } = 0;
        public string? Numero { get; set; }
        public string? Nome { get; set; }
    }

    public class TemaNewRequest
    {
        public string? Numero { get; set; }
        public string? Nome { get; set; }
    }

    public class TemaUpdateRequest
    {
        public int Id { get; set; }
        public string? Numero { get; set; }
        public string? Nome { get; set; }
    }
    #endregion

    #region Orador
    public class OradorQueryRequest
    {
        public string? Orador { get; set; }
        public Privilegio? Privilegio { get; set; }
        public string? Tema { get; set; }
        public string? Selecionados { get; set; }
        public bool? Publico { get; set; }
        public bool? Preparado { get; set; }
    }
    public class OradorNewRequest
    {
        public int PubId { get; set; }
        public bool Publico { get; set; }
    }

    public class OradorUpdateRequest
    {
        public int? PubId { get; set; }
        public bool Publico { get; set; }
    }
    #endregion

    #region OradorTema
    public class OradorTemaQueryRequest
    {
        public int? Id { get; set; }
        public int? PubId { get; set; }
        public int? TemaId { get; set; }
        public bool? Publico { get; set; }
    }

    public class OradorTemaNewRequest
    {
        public int PubId { get; set; }
        public int TemaId { get; set; }
        public bool Publico { get; set; }
    }
    public class OradorTemaUpdateRequest
    {
        public int ID { get; set; }
        public int PubId { get; set; }
        public int TemaId { get; set; }
        public bool Publico { get; set; }
    }

    #endregion

    #region Discurso
    public class DiscursoQueryRequest
    {
        public int? ArranjoId { get; set; }
        public int? CongId { get; set; }
        public string? Numero { get; set; }
        public string? Tema { get; set; }
        public string? Mes { get; set; }
        public string? Ano { get; set; }
        public string? Orador { get; set; }
        public bool? Recebido { get; set; }
    }

    public class DiscursoNewRequest
    {
        public int ArranjoId { get; set; }
        public string? Data { get; set; }
        public string? Orador { get; set; }
        public string? Contato { get; set; }
        public int TemaId { get; set; }
        public bool Recebido { get; set; }
        public string Status { get; set; }
        public string? Obs { get; set; }
    }

    public class DiscursoUpdateRequest
    {
        public int ID { get; set; }
        public int ArranjoId { get; set; }
        public int TemaId { get; set; }
        public string? Data { get; set; }
        public string? Orador { get; set; }
        public string? Contato { get; set; }
        public bool Recebido { get; set; }
        public string? Status { get; set; }
        public string? Obs { get; set; }
    }

    #endregion

    #region OradorTema

    
    #endregion
}
