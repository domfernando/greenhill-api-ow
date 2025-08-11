using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Application.Util
{
    public class Reply
    {
        public Reply()
        {
            Success = true;
            Validate = true;
            Errors = new List<string>();
            Messages = new List<string>();
        }
        public System.Net.HttpStatusCode Status { get; set; }
        public bool Success { get; set; }
        public bool Validate { get; set; }
        public Object Data { get; set; }
        public List<string> Messages { get; set; }
        public List<string> Errors { get; set; }
    }
}
