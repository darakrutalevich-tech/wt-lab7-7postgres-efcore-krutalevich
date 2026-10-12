using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_krutalevich.Models;

namespace wt_lab7_7postgres_efcore_krutalevich.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Flight> Flights => Set<Flight>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flight>().HasData(
                new Flight
                {
                    Id = 1,
                    FlightNumber = "AV-101",
                    Departure = "Минск",
                    Arrival = "Варшава",
                    Price = 250,
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Flight
                {
                    Id = 2,
                    FlightNumber = "AV-202",
                    Departure = "Минск",
                    Arrival = "Берлин",
                    Price = 320,
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
                },
                new Flight
                {
                    Id = 3,
                    FlightNumber = "AV-303",
                    Departure = "Гомель",
                    Arrival = "Москва",
                    Price = 180,
                    IsAvailable = false,
                    CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
