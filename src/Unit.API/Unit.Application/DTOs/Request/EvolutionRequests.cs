using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.Models;

namespace Unit.Application.DTOs.Request
{
    public class SendEvolutionMessageRequest
    {
        public string Instance { get; set; }
        public string Number { get; set; }
        public TextMessage TextMessage { get; set; }
    }

    public class SendEvolutionMessageWithLocationRequest
    {
        public string Instance { get; set; }
        public string Number { get; set; }
        public TextMessage TextMessage { get; set; }
        public LocationMessage Location { get; set; }
    }

    public class TextMessage
    {
        public string Text { get; set; }
    }

    public class LocationMessage
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
