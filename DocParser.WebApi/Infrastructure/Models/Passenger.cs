namespace DocParser.WebApi.Infrastructure.Models;

public class Passenger : BaseEntity
{
    public double? TotalSum { get; set; }
    public string? TotalCurrency { get; set; }
    public string? PersonName { get; set; }
    public string? PersonSurname { get; set; }
    public string? PassportNumber { get; set; }
    public string? MealType { get; set; }
    public string? Insurance { get; set; }
    public string? CabinBaggageType { get; set; }
    public string? BookingReference { get; set; }
    public string? BookingConfirmStatus { get; set; }
    public string? BaggageType { get; set; }
    public string? Baggage { get; set; }
   
}