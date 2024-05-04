using System.Text.Json.Serialization;

namespace DocParser.WebApi.Models;

//https://app.quicktype.io/
public class AirplaneTicket
{
    [JsonPropertyName("flights")]
    public List<Flight>? Flights { get; set; }

    [JsonPropertyName("passengers")]
    public List<Passenger>? Passengers { get; set; }

    public class Flight
    {
        [JsonPropertyName("airlineIATA")]
        public string? AirlineIata { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("airlineNameByIATA")]
        public string? AirlineNameByIata { get; set; }

        [JsonPropertyName("arrivalCity")]
        public string? ArrivalCity { get; set; }

        [JsonPropertyName("arrivalCountry")]
        public string? ArrivalCountry { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("arrivalDateTime")]
        public DateTime? ArrivalDateTime { get; set; }

        [JsonPropertyName("arrivalIATA")]
        public string? ArrivalIata { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("arrivalTerminal")]
        public string? ArrivalTerminal { get; set; }

        [JsonPropertyName("departureCity")]
        public string? DepartureCity { get; set; }

        [JsonPropertyName("departureCountry")]
        public string? DepartureCountry { get; set; }

        [JsonPropertyName("departureDateTime")]
        public DateTime? DepartureDateTime { get; set; }

        [JsonPropertyName("departureIATA")]
        public string? DepartureIata { get; set; }

        [JsonPropertyName("departureTerminal")]
        public string? DepartureTerminal { get; set; }

        [JsonPropertyName("flightNumber")]
        public double FlightNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("stopover")]
        public string? Stopover { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("warningsRestrictions")]
        public string? WarningsRestrictions { get; set; }
    }

    public class Passenger
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("baggage")]
        public string? Baggage { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("baggageType")]
        public string? BaggageType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("bookingConfirmStatus")]
        public string? BookingConfirmStatus { get; set; }

        [JsonPropertyName("bookingId")]
        public string? BookingId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("bookingReference")]
        public string? BookingReference { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("cabinBaggageType")]
        public string? CabinBaggageType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("insurance")]
        public string? Insurance { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("mealType")]
        public string? MealType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("passportNumber")]
        public string? PassportNumber { get; set; }

        [JsonPropertyName("person")]
        public Person? Person { get; set; }

        [JsonPropertyName("PNR")]
        public string? Pnr { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("refundableStatus")]
        public bool? RefundableStatus { get; set; }

        [JsonPropertyName("ticketNumber")]
        public string? TicketNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("totalCurrency")]
        public string? TotalCurrency { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("totalSum")]
        public double? TotalSum { get; set; }
    }

    public class Person
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }
    }
    
}
