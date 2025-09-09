using Unit.Application.Enums;

namespace Unit.Application.DTOs.Request
{
    public class PubQueryModel
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Privilegio { get; set; }
        public string? Situacao { get; set; }
        public string? Genero { get; set; }
        public string? Faixa { get; set; }
        public string? Varao { get; set; }
        public string? Dianteira { get; set; }
        public string? Escola { get; set; }
        public bool? Orador { get; set; }
    }

    public class PubNewModel
    {
        public string? Nome { get; set; }
    }

    public class PubUpdateModel
    {
        public int ID { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Nome { get; set; }
        public string? Situacao { get; set; }
        public string? Privilegio { get; set; }
        public bool? Orador { get; set; } = false;
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Nascimento { get; set; }
        public string? Batismo { get; set; }
        public string? AuxiliarAte { get; set; }
        public string? Genero { get; set; }
        public string? Obs { get; set; }
        public string? EmergenciaNome { get; set; }
        public string? EmergenciaContato { get; set; }
        public int? Idade { get; set; }
        public int? TempoBatismo { get; set; } = 0;
        public bool? Escola { get; set; } = false;
        public bool? Notificar { get; set; } = false;
        public int? UsuarioId { get; set; } = 0;
        public string? PapeisSelecionados { get; set; }
        public string? PapeisRemovidos { get; set; }
    }
}
