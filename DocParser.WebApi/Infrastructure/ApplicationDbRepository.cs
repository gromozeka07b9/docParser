using System.Collections.Immutable;
using DocParser.WebApi.Integration;
using DocParser.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DocParser.WebApi.Infrastructure;

public class ApplicationDbRepository
{
    private readonly ApplicationDbContext dbContext;

    public ApplicationDbRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    public void Dispose()
    {
        dbContext.Dispose();
    }
    
    public async Task<bool> IsExistByIdentityAsync(string userIdentity, string identity, DocumentTypes documentType)
    {
        if (documentType == DocumentTypes.AirplaneTicket)
        {
            return await dbContext.AirplaneTickets.AnyAsync(t => t.Identity == identity && t.UserIdentity == userIdentity);
        }
        throw new Exception("Not supported document type");
    }
    
    public Task AddPassengerAsync(AirplaneTicket.Passenger passenger, Guid passengerId,  string identity, string userIdentity, CancellationToken cancellationToken)
    {
        if (passenger == null) throw new Exception("Passenger is null");

        Infrastructure.Models.Passenger dbEntity = new Models.Passenger()
        {
            Id = passengerId,
            Created = DateTime.UtcNow,
            UserIdentity = userIdentity,
            Identity = identity,
            Baggage = passenger.Baggage,
            Insurance = passenger.Insurance,
            BaggageType = passenger.BaggageType,
            BookingReference = passenger.BookingReference,
            MealType = passenger.MealType,
            PassportNumber = passenger.PassportNumber,
            BookingConfirmStatus = passenger.BookingConfirmStatus,
            CabinBaggageType = passenger.CabinBaggageType,
            PersonName = passenger.Person?.Name,
            PersonSurname = passenger.Person?.Surname,
            TotalCurrency = passenger.TotalCurrency,
            TotalSum = passenger.TotalSum
        };
        dbContext.Passengers.Add(dbEntity);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task AddAirplaneTicketAsync(AirplaneTicket.Flight flight, string identity, Guid passengerId, string userIdentity, CancellationToken cancellationToken)
    {
        if (flight == null) throw new Exception("Flight is null");

        Infrastructure.Models.AirplaneTicket dbEntity = new Models.AirplaneTicket()
        {
            Created = DateTime.UtcNow,
            PassengerId = passengerId,
            UserIdentity = userIdentity,
            Identity = identity,
            TimelineStartDateTime = flight.DepartureDateTime ?? default,
            TimelineEndDateTime = flight.ArrivalDateTime ?? default,
            DepartureCountry = flight.DepartureCountry,
            DepartureCity = flight.DepartureCity,
            ArrivalCountry = flight.ArrivalCountry,
            ArrivalCity = flight.ArrivalCity,
            DepartureIata = flight.DepartureIata,
            ArrivalIata = flight.ArrivalIata,
            AirlineNameByIata = flight.AirlineNameByIata,
            DepartureDateTime = flight.DepartureDateTime,
            DepartureTerminal = flight.DepartureTerminal,
            ArrivalDateTime = flight.ArrivalDateTime,
            ArrivalTerminal = flight.ArrivalTerminal,
            FlightNumber = flight.FlightNumber.ToString(),//double нужен в модели для gpt чтобы исключить попадание мусора в виде строк, но далее нужна именно строка
            WarningsRestrictions = flight.WarningsRestrictions,
            Stopover = flight.Stopover
        };
        dbContext.AirplaneTickets.Add(dbEntity);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Models.AirplaneTicket>> GetAirplaneTickets(string userIdentity)
    {
        if (string.IsNullOrEmpty(userIdentity)) throw new Exception("userIdentity is null");
        //var tickets = dbContext.AirplaneTickets.Where(t => t.UserIdentity == userIdentity).ToList();
        var tickets =
            await (from ticket in dbContext.AirplaneTickets where ticket.UserIdentity == userIdentity orderby ticket.TimelineStartDateTime descending select ticket)
                .ToListAsync(CancellationToken.None).ConfigureAwait(false);
        return tickets;
    }

}