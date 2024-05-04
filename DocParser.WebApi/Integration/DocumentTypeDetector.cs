using System.Text;
using DocParser.WebApi.Queries;

namespace DocParser.WebApi.Integration;

public class DocumentTypeDetector
{
    private readonly PdfParser pdfParser;
    private readonly IServiceScopeFactory scopeFactory;

    public DocumentTypeDetector(PdfParser pdfParser, IServiceScopeFactory scopeFactory)
    {
        this.pdfParser = pdfParser;
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }
    public async Task<QueryResult<DocumentTypes>> DetectTypeAsync(string filePath)
    {
        string resultText = pdfParser.GetTextFromPdf(filePath);
        string dateTimeForLog = $"{DateTime.Now.Month}{DateTime.Now.Day}_{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
        Directory.CreateDirectory("data");
        await File.WriteAllTextAsync(Path.Combine("data", $"{dateTimeForLog}_pdf.txt"), resultText, Encoding.UTF8);

        using (var scope = scopeFactory.CreateScope())
        {
            var getDocumentTypeQuery = scope.ServiceProvider.GetService<GetDocumentTypeQuery>() ?? throw new ArgumentNullException($"{nameof(GetDocumentTypeQuery)} can't be undefined.");
            var queryResult = await getDocumentTypeQuery.GetDocumentTypeAsync(resultText);
            return queryResult;
        }        
    }
}