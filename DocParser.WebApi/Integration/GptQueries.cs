using System.Net.Mime;
using System.Text;
using System.Text.Json;
using DocParser.WebApi.Models;

namespace DocParser.WebApi.Integration;

public partial class GptQueries
{
    private readonly GptSettings gptSettings;
    //private readonly ApplicationContext dbContext;

    public GptQueries(GptSettings gptSettings)
    {
        this.gptSettings = gptSettings;
        //this.dbContext = dbContext;
    }
    public async Task<DocumentTypes> GetDocumentTypeFromGptAsync(string textForAnalyze)
    {
        string shortText = GetShortString(textForAnalyze);
        string documentTypes = string.Join(",", Enum.GetNames(typeof(DocumentTypes)));
        GptRequest gptRequest = new GptRequest()
        {
            model = gptSettings.Model,
            max_tokens = gptSettings.Max_Tokens,
            temperature = gptSettings.Temperature,
            response_format = new GptRequest.ResponseFormat() { type = "json_object" },
            messages = new List<GptRequest.Message>()
            {
                new GptRequest.Message() { content = "You are a information analist.", role = "system" },
                new GptRequest.Message() { content = shortText, role = "user" },
                new GptRequest.Message() { content = (new GptPrompt(){Text = "detect document type and choose from possible types and return json:" + documentTypes}).Text, role = "user"}
            }
        };
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", gptSettings.AuthorizationToken);
        var contentHttp = new StringContent(JsonSerializer.Serialize<GptRequest>(gptRequest), Encoding.UTF8, MediaTypeNames.Application.Json);
        var result = await client.PostAsync(gptSettings.ApiEndpoint, contentHttp);
        var response = await result.Content.ReadAsStringAsync();
        var gptJsonResponse = JsonSerializer.Deserialize<GptResponse>(response);
        if (gptJsonResponse != null)
        {
            if (gptJsonResponse.error?.code != null)
            {
                throw new ApplicationException("Error while get document type:" + gptJsonResponse.error.code);
            }
            var gptDocumentTypeResponse = JsonSerializer.Deserialize<DocumentTypeResponse>(gptJsonResponse.choices[0].message.content, new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
            if (Enum.TryParse(gptDocumentTypeResponse?.DocumentType, true, out DocumentTypes documentType))
            {
                return documentType;
            }
        }

        return default;
    }

    private string GetShortString(string textForAnalyze)
    {
        int maxSpacesCount = 30;
        int spacesCount = 0;
        for (int i = 0; i < textForAnalyze.Length; i++)
        {
            if (textForAnalyze[i] == ' ')
            {
                spacesCount++;
            }
            if(spacesCount >= maxSpacesCount) return textForAnalyze.Substring(0, i);
        }

        return textForAnalyze;
    }

    public async Task<string> GetJsonFromGptAsync(string textForAnalyze, DocumentTypes documentType)
    {
        GptRequest gptRequest = new GptRequest()
        {
            model = gptSettings.Model,
            max_tokens = gptSettings.Max_Tokens,
            temperature = gptSettings.Temperature,
            response_format = new GptRequest.ResponseFormat() { type = "json_object" },
            messages = new List<GptRequest.Message>()
            {
                new GptRequest.Message() { content = "You are a information analist.", role = "system" },
                new GptRequest.Message() { content = textForAnalyze, role = "user" },
            }
        };
        /*if (prompts.Any())
        {
            gptRequest.messages.AddRange(prompts.Select(p=>new GptRequest.Message(){role = "user", content = p.Text}));
        }
        else*/
        {
            //var schemaText = GetSchemaText(documentType.ToString() + ".txt");
            var schemaText = GetSchemaText(documentType.ToString() + ".json");
            var gptMessage = new GptRequest.Message()
            {
                //role = "user", content = "use English language for departureCity, departureCountry, arrivalCity, arrivalCountry, warningsRestrictions, bookingConfirmStatus, mealType, cabinBaggageType and describe information in json by schema:" + schemaText
                role = "user", content = "describe information in json by schema and translate to English and empty fields not include to response:" + schemaText
            };
            gptRequest.messages.Add(gptMessage);
        }
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", gptSettings.AuthorizationToken);
        var contentHttp = new StringContent(JsonSerializer.Serialize<GptRequest>(gptRequest), Encoding.UTF8, MediaTypeNames.Application.Json);
        var result = await client.PostAsync(gptSettings.ApiEndpoint, contentHttp);
        return await result.Content.ReadAsStringAsync();
    }

    private string GetSchemaText(string filename)
    {
        return File.ReadAllText(Path.Combine("Schemas", filename));
    }
}

public class DocumentTypeResponse
{
    public string DocumentType { get; set; }
}