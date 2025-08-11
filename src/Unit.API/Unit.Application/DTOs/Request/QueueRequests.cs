namespace Unit.Application.DTOs.Request
{
    public class QueryQueueRequest
    {
        public int? Id { get; set; }
        public string? Source { get; set; }
        public int MessageMode { get; set; }
        public string? Instance { get; set; }
        public bool? Processed { get; set; }
        public bool? Enabled { get; set; }
        public bool? Success { get; set; }
    }

    public class QueryQueueForProcess
    {
        public int MessageMode { get; set; }
        public int Quantity { get; set; } = 10;
    }

    public class  CreateQueueRequest
    {
        public string Source { get; set; }
        public int MessageMode { get; set; }
        public string Instance { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Message { get; set; }
        public string SendDate { get; set; }
    }

    public class UpdateQueueRequest : CreateQueueRequest
    {
        public int Id { get; set; }        
        public bool processed { get; set; }
        public bool enabled { get; set; }
        public bool success { get; set; }
        public int attempts { get; set; } = 0;
        public string log { get; set; }
    }

    public class QueryQueueGraphicsRequest
    {
        public string? Inicio { get; set; }
        public string? Fim { get; set; }
    }
}
