using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_krutalevich.Data;
using wt_lab7_7postgres_efcore_krutalevich.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Блок 1, уровень 2: проверка запуска
app.MapGet("/", () => "Minimal API работает!");

// Блок 1, уровень 3: информация о приложении
app.MapGet("/api/info", () =>
{
    return new
    {
        Application = "Web Technologies",
        Technology = "ASP.NET Core Minimal API",
        Variant = "Авиакомпания",
        Version = "1.1",
        Updated = DateTime.UtcNow
    };
});

//// Блок 3, уровень 1: список всех рейсов
//app.MapGet("/api/flights", () =>
//{
//    return Results.Ok(FlightStore.Flights);
//});

//Блок 3, уровень 2: рейс по Id
app.MapGet("/api/flights/{id}", (int id) =>
{
    var flight = FlightStore.Flights.FirstOrDefault(f => f.Id == id);
    if (flight == null)
        return Results.NotFound();

    return Results.Ok(flight);
});

// Блок 3, уровень 3: список рейсов + фильтрация (query-параметр)
app.MapGet("/api/flights", (string? search) =>
{
    var flights = FlightStore.Flights.AsEnumerable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        flights = flights.Where(f =>
            f.FlightNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            f.Departure.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            f.Arrival.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    return Results.Ok(flights);
});

// Блок 4: POST /api/flights
app.MapPost("/api/flights", (Flight flight) =>
{
    // Простая валидация вручную ( для блока 6)
    if (string.IsNullOrWhiteSpace(flight.FlightNumber) ||
        string.IsNullOrWhiteSpace(flight.Departure) ||
        string.IsNullOrWhiteSpace(flight.Arrival) ||
        flight.Price <= 0)
    {
        return Results.BadRequest(new
        {
            error = "Некорректные данные: номер рейса, города и цена обязательны; цена > 0."
        });
    }

    flight.Id = FlightStore.NextId();
    FlightStore.Flights.Add(flight);
    return Results.Created($"/api/flights/{flight.Id}", flight);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Блок 5, уровень 1: PUT /api/flights/{id}
app.MapPut("/api/flights/{id}", (int id, Flight updated) =>
{
    var flight = FlightStore.Flights.FirstOrDefault(f => f.Id == id);
    if (flight == null)
        return Results.NotFound();

    // для блока 6
    if (string.IsNullOrWhiteSpace(updated.FlightNumber) ||
        string.IsNullOrWhiteSpace(updated.Departure) ||
        string.IsNullOrWhiteSpace(updated.Arrival) ||
        updated.Price <= 0)
    {
        return Results.BadRequest(new
        {
            error = "Некорректные данные: номер рейса, города и цена обязательны; цена > 0."
        });
    }

    flight.FlightNumber = updated.FlightNumber;
    flight.Departure = updated.Departure;
    flight.Arrival = updated.Arrival;
    flight.Price = updated.Price;
    flight.IsAvailable = updated.IsAvailable;

    return Results.Ok(flight);
});

// Блок 5, уровень 2: DELETE /api/flights/{id}
app.MapDelete("/api/flights/{id}", (int id) =>
{
    var flight = FlightStore.Flights.FirstOrDefault(f => f.Id == id);
    if (flight == null)
        return Results.NotFound();

    FlightStore.Flights.Remove(flight);
    return Results.NoContent();
});

//var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
//app.Run($"http://0.0.0.0:{port}");
app.Run();