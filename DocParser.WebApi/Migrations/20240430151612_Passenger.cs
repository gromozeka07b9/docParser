using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocParser.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Passenger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirplaneTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimelineStartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimelineEndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SourceDocumentLink = table.Column<string>(type: "TEXT", nullable: true),
                    PassengerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FlightNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AirlineNameByIata = table.Column<string>(type: "TEXT", nullable: true),
                    Stopover = table.Column<string>(type: "TEXT", nullable: true),
                    WarningsRestrictions = table.Column<string>(type: "TEXT", nullable: true),
                    DepartureCity = table.Column<string>(type: "TEXT", nullable: true),
                    DepartureCountry = table.Column<string>(type: "TEXT", nullable: true),
                    DepartureIata = table.Column<string>(type: "TEXT", nullable: true),
                    DepartureTerminal = table.Column<string>(type: "TEXT", nullable: true),
                    DepartureDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArrivalCity = table.Column<string>(type: "TEXT", nullable: true),
                    ArrivalCountry = table.Column<string>(type: "TEXT", nullable: true),
                    ArrivalIata = table.Column<string>(type: "TEXT", nullable: true),
                    ArrivalDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArrivalTerminal = table.Column<string>(type: "TEXT", nullable: true),
                    UserIdentity = table.Column<string>(type: "TEXT", nullable: false),
                    Identity = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirplaneTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalSum = table.Column<double>(type: "REAL", nullable: true),
                    TotalCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    PersonName = table.Column<string>(type: "TEXT", nullable: true),
                    PersonSurname = table.Column<string>(type: "TEXT", nullable: true),
                    PassportNumber = table.Column<string>(type: "TEXT", nullable: true),
                    MealType = table.Column<string>(type: "TEXT", nullable: true),
                    Insurance = table.Column<string>(type: "TEXT", nullable: true),
                    CabinBaggageType = table.Column<string>(type: "TEXT", nullable: true),
                    BookingReference = table.Column<string>(type: "TEXT", nullable: true),
                    BookingConfirmStatus = table.Column<string>(type: "TEXT", nullable: true),
                    BaggageType = table.Column<string>(type: "TEXT", nullable: true),
                    Baggage = table.Column<string>(type: "TEXT", nullable: true),
                    UserIdentity = table.Column<string>(type: "TEXT", nullable: false),
                    Identity = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirplaneTickets_Id",
                table: "AirplaneTickets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AirplaneTickets_PassengerId",
                table: "AirplaneTickets",
                column: "PassengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_Id",
                table: "Passengers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirplaneTickets");

            migrationBuilder.DropTable(
                name: "Passengers");
        }
    }
}
