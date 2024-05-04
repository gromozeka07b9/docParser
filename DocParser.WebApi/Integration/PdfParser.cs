using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace DocParser.WebApi.Integration;

public class PdfParser
{
    public string GetTextFromPdf(string pdfFilename)
    {
        StringBuilder sb = new StringBuilder();
        using (var pdf = PdfDocument.Open(pdfFilename))
        {
            foreach (var page in pdf.GetPages())
            {
                var text = ContentOrderTextExtractor.GetText(page);
                var otherText = string.Join(" ", page.GetWords());
                sb.AppendLine(text);
            }
        }
        return sb.ToString().ReplaceLineEndings($"\\n");
    }
}