using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Domain.Entities.Extra
{
    public class Queue : EntidadeBase
    {
        public string? Source { get; set; }
        public int MessageMode { get; set; }
        public string? Instance { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Message { get; set; }
        public DateTime? SendDate { get; set; } = DateTime.MinValue;
        public bool? processed { get; set; }
        public bool? enabled { get; set; }
        public bool? success { get; set; }
        public int? attempts { get; set; } = 0;
        public string? log { get; set; }

        [NotMapped]
        public string SendDateFormatted
        {
            get
            {
                return string.Format("{0:dd/MM/yyyy}", SendDate);
            }
        }
        [NotMapped]
        public string SendDateTimeFormatted
        {
            get
            {
                return string.Format("{0:dd/MM/yyyy HH:MM}", SendDate);
            }
        }
        [NotMapped]
        public string Status
        {
            get
            {
                string status = string.Empty;

                switch (processed)
                {
                    case true:
                        if (success == true)
                        {
                            status += "Sucesso";
                        }
                        else
                        {
                            status += "Falha";
                        }
                        break;

                    default:
                        if (enabled == true && success == false)
                        {
                            status = "Pendente";
                        }
                        else if (enabled == false)
                        {
                            status = "Descartado";
                        }
                        break;
                }

                return status;
            }
        }
    }
}
