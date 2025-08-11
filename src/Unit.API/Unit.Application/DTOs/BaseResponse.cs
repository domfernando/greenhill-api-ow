using System.ComponentModel.DataAnnotations.Schema;

namespace Unit.Application.DTOs
{
    public class BaseResponse
    {
        public int Id { get; set; }
        public DateTime? Criado { get; set; }
        public DateTime? Alterado { get; set; }
        
        public string CriadoFormatado
        {
            get
            {
                return Criado.HasValue ? Criado.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            }
        }

        [NotMapped]
        public string AlteradoFormatado
        {
            get
            {
                return Alterado.HasValue ? Alterado.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            }
        }       
    }
}
