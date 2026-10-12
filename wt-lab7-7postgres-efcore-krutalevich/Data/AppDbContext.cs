using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_krutalevich.Models;

namespace wt_lab7_7postgres_efcore_krutalevich.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Flight> Flights => Set<Flight>();
    }
}
