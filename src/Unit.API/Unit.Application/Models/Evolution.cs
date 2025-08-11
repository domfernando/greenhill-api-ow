namespace Unit.Application.Models
{
    public class MessageDTO
    {
        public string Instance { get; set; }
        public string Number { get; set; }
        public TextMessage TextMessage { get; set; }

    }

    public class TextMessage
    {
        public string Text { get; set; }
    }

    public class LocactionDTO
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class MessageWithLocationDTO
    {
        public string Instance { get; set; }
        public string Number { get; set; }
        public TextMessage TextMessage { get; set; }
        public LocactionDTO Location { get; set; }
    }


    public class Rootobject
    {
        public string number { get; set; }
        public Locationmessage locationMessage { get; set; }
        public Options options { get; set; }
    }

    public class Locationmessage
    {
        public string name { get; set; }
        public string address { get; set; }
        public int latitude { get; set; }
        public int longitude { get; set; }
    }

    public class Options
    {
        public int delay { get; set; }
        public string presence { get; set; }
        public bool linkPreview { get; set; }
        public Quoted quoted { get; set; }
        public Mentions mentions { get; set; }
    }

    public class Quoted
    {
        public Key key { get; set; }
        public Message message { get; set; }
    }

    public class Key
    {
        public string remoteJid { get; set; }
        public bool fromMe { get; set; }
        public string id { get; set; }
    }

    public class Message
    {
        public string conversation { get; set; }
    }

    public class Mentions
    {
        public bool everyone { get; set; }
        public string[] mentioned { get; set; }
    }

}
