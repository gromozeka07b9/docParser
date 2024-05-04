using DocParser.WebApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DocParser.WebApi;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<AirplaneTicket> AirplaneTickets => Set<AirplaneTicket>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    
    public ApplicationDbContext() => Database.EnsureCreated();
 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=storage.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AirplaneTicket>().HasIndex(ticket => ticket.Id);
        modelBuilder.Entity<Passenger>().HasIndex(passenger => passenger.Id);
        modelBuilder.Entity<AirplaneTicket>().HasIndex(ticket => ticket.PassengerId);
    }
}