using DocParser.WebApi.Integration;

namespace DocParser.WebApi.Queries;

public class GetDocumentTypeQuery
{
    private readonly GptQueries gptQueries;

    public GetDocumentTypeQuery(GptQueries gptQueries)
    {
        this.gptQueries = gptQueries;
    }
    
    public async Task<QueryResult<DocumentTypes>> GetDocumentTypeAsync(string text)
    {
        DocumentTypes documentType = DocumentTypes.Other;
        try
        {
            documentType = await gptQueries.GetDocumentTypeFromGptAsync(text);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new QueryResult<DocumentTypes>(documentType){Error = e.Message};
        }
        return new QueryResult<DocumentTypes>(documentType);
    }
}