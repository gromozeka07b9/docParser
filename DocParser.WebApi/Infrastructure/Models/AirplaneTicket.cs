namespace DocParser.WebApi.Infrastructure.Models;

public class AirplaneTicket : BaseEntity
{
    //common
    public DateTime TimelineStartDateTime { get; set; }
    public DateTime TimelineEndDateTime { get; set; }
    public string? SourceDocumentLink { get; set; } 
    public Guid? PassengerId { get; set; } 
    
    //flight
    public string? FlightNumber { get; set; }
    public string? AirlineNameByIata { get; set; }
    public string? Stopover { get; set; }
    public string? WarningsRestrictions { get; set; }
    
    //departure
    public string? DepartureCity { get; set; }
    public string? DepartureCountry { get; set; }
    public string? DepartureIata { get; set; }
    public string? DepartureTerminal { get; set; }
    public DateTime? DepartureDateTime { get; set; }
    
    //arrival
    public string? ArrivalCity { get; set; }
    public string? ArrivalCountry { get; set; }
    public string? ArrivalIata { get; set; }
    public DateTime? ArrivalDateTime { get; set; }
    public string? ArrivalTerminal { get; set; }
}