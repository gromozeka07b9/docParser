namespace DocParser.WebApi.Models;

public class GptSettings
{
    public string PromptText { get; set; }
    public string AuthorizationToken { get; set; }
    public string ApiEndpoint { get; set; }
    public string Model { get; set; }
    public int Max_Tokens { get; set; }
    public int Temperature { get; set; }
}