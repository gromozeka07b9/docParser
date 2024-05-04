﻿// <auto-generated />
using System;
using DocParser.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DocParser.WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("DocParser.WebApi.Infrastructure.Models.AirplaneTicket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AirlineNameByIata")
                        .HasColumnType("TEXT");

                    b.Property<string>("ArrivalCity")
                        .HasColumnType("TEXT");

                    b.Property<string>("ArrivalCountry")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ArrivalDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ArrivalIata")
                        .HasColumnType("TEXT");

                    b.Property<string>("ArrivalTerminal")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("DepartureCity")
                        .HasColumnType("TEXT");

                    b.Property<string>("DepartureCountry")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DepartureDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("DepartureIata")
                        .HasColumnType("TEXT");

                    b.Property<string>("DepartureTerminal")
                        .HasColumnType("TEXT");

                    b.Property<string>("FlightNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PassengerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceDocumentLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("Stopover")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimelineEndDateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimelineStartDateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserIdentity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WarningsRestrictions")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("PassengerId");

                    b.ToTable("AirplaneTickets");
                });

            modelBuilder.Entity("DocParser.WebApi.Infrastructure.Models.Passenger", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Baggage")
                        .HasColumnType("TEXT");

                    b.Property<string>("BaggageType")
                        .HasColumnType("TEXT");

                    b.Property<string>("BookingConfirmStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("BookingReference")
                        .HasColumnType("TEXT");

                    b.Property<string>("CabinBaggageType")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Insurance")
                        .HasColumnType("TEXT");

                    b.Property<string>("MealType")
                        .HasColumnType("TEXT");

                    b.Property<string>("PassportNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("PersonName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PersonSurname")
                        .HasColumnType("TEXT");

                    b.Property<string>("TotalCurrency")
                        .HasColumnType("TEXT");

                    b.Property<double?>("TotalSum")
                        .HasColumnType("REAL");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserIdentity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Passengers");
                });
#pragma warning restore 612, 618
        }
    }
}
