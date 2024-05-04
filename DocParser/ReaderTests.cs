using System.Net.Mime;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace DocParser;

public class ReaderTests
{
    [Test]
    public async Task SaveText()
    {
        //await SaveTextFromPdf("fly1.pdf");
        //SaveTextFromPdf("fly2.pdf");
        //SaveTextFromPdf("fly3.pdf");
        //SaveTextFromPdf("fly4.pdf");
        //SaveTextFromPdf("fly5.pdf");
        await SaveTextFromPdf("train1.pdf");
    }

    static async Task SendToGpt(string textForAnalyze)
    {
        string textTask1 = "describe information in json with parameters: flight count, for each flight: departure date and time, arrival date and time, departure place in IATA format, arrival place in IATA format, person name with splitted name and surname, flight number splitted to owner and number, booking reference";
        var json = $$"""{"model":"gpt-3.5-turbo-1106","max_tokens":4096,"temperature":0,"response_format":{"type":"json_object"},"messages":[{"role":"system","content":"You are a information analist."},{"role":"user","content":"{{textForAnalyze}}"},{"role":"user","content":"{{textTask1}}"}]}""";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        //client.DefaultRequestHeaders.Add("Authorization", "set token here");
        throw new Exception("Need set token!");
        var contentHttp = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var result = await client.PostAsync("https://api.openai.com/v1/chat/completions", contentHttp);
        string answerGpt = await result.Content.ReadAsStringAsync();
        Console.WriteLine(answerGpt);
    }
    static async Task SaveTextFromPdf(string pdfFilename)
    {
        if(!pdfFilename.Contains(".pdf")) return;
    
        StringBuilder sb = new StringBuilder();
        using (var pdf = PdfDocument.Open(pdfFilename))
        {
            foreach (var page in pdf.GetPages())
            {
                // Either extract based on order in the underlying document with newlines and spaces.
                //ContentOrderTextExtractor.Options
                var text = ContentOrderTextExtractor.GetText(page);

                // Or based on grouping letters into words.
                var otherText = string.Join(" ", page.GetWords());

                // Or the raw text of the page's content stream.
                //var rawText = page.Text;
                sb.AppendLine(text);
                //Console.WriteLine(text);
            }
        }

        string finalText = sb.ToString().ReplaceLineEndings($"\\n");
        Console.WriteLine(finalText);
        File.WriteAllText(pdfFilename+ ".txt", finalText);

        await SendToGpt(finalText);
    }
}
