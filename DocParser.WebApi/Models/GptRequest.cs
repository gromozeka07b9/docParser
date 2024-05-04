namespace DocParser.WebApi.Models;

public class GptRequest
{
    public string model { get; set; }
    public int max_tokens { get; set; }
    public int temperature { get; set; }
    public ResponseFormat response_format { get; set; }
    public List<Message> messages { get; set; }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ResponseFormat
    {
        public string type { get; set; }
    }

}

