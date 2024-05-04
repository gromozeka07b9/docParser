using System.Text;
using System.Text.Json;
using DocParser.WebApi.Infrastructure;
using DocParser.WebApi.Integration;
using DocParser.WebApi.Models;

namespace DocParser.WebApi.Commands;

public class ProcessAirplaneTicketCommand : GptCommand
{
    private readonly PdfParser pdfParser;
    private readonly IServiceScopeFactory scopeFactory;

    public ProcessAirplaneTicketCommand(GptQueries gptQueries, PdfParser pdfParser, IServiceScopeFactory scopeFactory) : base(gptQueries)
    {
        this.pdfParser = pdfParser;
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }
    
    public async Task<CommandResult> Execute(string userIdentity, string filePath)
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var applicationDbRepository = scope.ServiceProvider.GetService<ApplicationDbRepository>() ?? throw new ArgumentNullException($"{nameof(ApplicationDbRepository)} can't be undefined.");
            string dateTimeForLog = $"{DateTime.Now.Month}{DateTime.Now.Day}_{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
            string resultText = pdfParser.GetTextFromPdf(filePath);
            string resultJson = await gptQueries.GetJsonFromGptAsync(resultText, DocumentTypes.AirplaneTicket);
            await File.WriteAllTextAsync(Path.Combine("data", $"{dateTimeForLog}_response.json"), resultJson, Encoding.UTF8);
            var gptResponse = JsonSerializer.Deserialize<GptResponse>(resultJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            string? prettyJson = gptResponse?.choices?.FirstOrDefault()!.message.content.Replace("\n", Environment.NewLine);
            if (prettyJson != null)
            {
                var airplaneTicket = JsonSerializer.Deserialize<AirplaneTicket>(prettyJson, new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
                if ((airplaneTicket != null) && (airplaneTicket?.Flights?.Count > 0))
                {
                    foreach (var flight in airplaneTicket.Flights)
                    {
                        AirplaneTicket.Passenger passenger = new AirplaneTicket.Passenger();
                        if (airplaneTicket?.Passengers?.Count > 0)
                        {
                            passenger = airplaneTicket.Passengers[0];
                        }

                        string passengerIdentity = !string.IsNullOrEmpty(passenger.PassportNumber)
                            ? passenger.PassportNumber
                            : passenger.Pnr ?? passenger.BookingId ?? Guid.NewGuid().ToString();
                        Guid passengerId = Guid.NewGuid();
                        await applicationDbRepository.AddPassengerAsync(passenger, passengerId, passengerIdentity, userIdentity, CancellationToken.None);
                        string ticketIdentity = GetAirplaneTicketIdentity(flight, passenger);
                        if (string.IsNullOrEmpty(ticketIdentity)) return new CommandResult() { Error = "Identity error", Successed = false };

                        if (await applicationDbRepository.IsExistByIdentityAsync(userIdentity, ticketIdentity, DocumentTypes.AirplaneTicket))
                        {
                            Console.WriteLine("Already exists");
                            continue;                        
                        }
                        await applicationDbRepository.AddAirplaneTicketAsync(flight, ticketIdentity, passengerId, userIdentity, CancellationToken.None);
                    }
                }
            }
            await File.WriteAllTextAsync(Path.Combine("data", $"{dateTimeForLog}_pretty.json"), prettyJson, Encoding.UTF8);
        }
        return new CommandResult() { Successed = true, Error = string.Empty };
    }

    string GetAirplaneTicketIdentity(AirplaneTicket.Flight flight, AirplaneTicket.Passenger passenger)
    {
        return $"{flight.DepartureIata}_{passenger.TicketNumber}_{passenger.BookingId}_{passenger.Person?.Name}_{passenger.Person?.Surname}";
    }
    
}

public class GptCommand
{
    protected readonly GptQueries gptQueries;

    public GptCommand(GptQueries gptQueries)
    {
        this.gptQueries = gptQueries;
    }
    
}