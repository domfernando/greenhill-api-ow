using Unit.Application.DTOs.Request;

namespace Unit.Application.DTOs.Response
{
    public class QueueResponse
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public int MessageMode { get; set; }
        public string Name { get; set; }
        public string Instance { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public string SendDate { get; set; }
        public bool processed { get; set; }
        public bool enabled { get; set; }
        public bool success { get; set; }
        public int attempts { get; set; }
        public string log { get; set; }
        public string SendDateFormatted { get; set; }
        public string SendDateTimeFormatted { get; set; }
    }
}
