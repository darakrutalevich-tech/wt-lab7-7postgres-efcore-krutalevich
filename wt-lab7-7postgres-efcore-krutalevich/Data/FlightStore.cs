using wt_lab7_7postgres_efcore_krutalevich.Models;

namespace wt_lab7_7postgres_efcore_krutalevich.Data
{
    public static class FlightStore
    {
        public static List<Flight> Flights { get; } = new()
    {
        new Flight { Id = 1, FlightNumber = "AV-101", Departure = "Минск",  Arrival = "Варшава", Price = 250m, IsAvailable = true  },
        new Flight { Id = 2, FlightNumber = "AV-202", Departure = "Минск",  Arrival = "Берлин",  Price = 320m, IsAvailable = true  },
        new Flight { Id = 3, FlightNumber = "AV-303", Departure = "Гомель", Arrival = "Москва",  Price = 180m, IsAvailable = false },
        new Flight { Id = 4, FlightNumber = "AV-404", Departure = "Брест",  Arrival = "Киев",    Price = 210m, IsAvailable = true  },
        new Flight { Id = 5, FlightNumber = "AV-505", Departure = "Минск",  Arrival = "Париж",   Price = 450m, IsAvailable = true  }
    };

        public static int NextId() =>
            Flights.Count == 0 ? 1 : Flights.Max(f => f.Id) + 1;
    }
}
