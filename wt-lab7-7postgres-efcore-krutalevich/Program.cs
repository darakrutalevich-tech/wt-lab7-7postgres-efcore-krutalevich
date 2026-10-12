using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_krutalevich.Data;
using wt_lab7_7postgres_efcore_krutalevich.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var databaseUrl = builder.Configuration["DATABASE_URL"];
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var dbPort = uri.Port > 0 ? uri.Port : 5432;

    connectionString =
        $"Host={uri.Host};" +
        $"Port={dbPort};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={Uri.UnescapeDataString(userInfo[0])};" +
        $"Password={Uri.UnescapeDataString(userInfo[1])};" +
        "SSL Mode=Disable";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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
app.MapGet("/api/flights/{id}", async (int id, AppDbContext db) =>
{
    var flight = await db.Flights.FirstOrDefaultAsync(f => f.Id == id);

    if (flight == null)
        return Results.NotFound();

    return Results.Ok(flight);
});

// лаба 7, пункт 15
app.MapGet("/api/flights/search", async (string? search, AppDbContext db) =>
{
    var query = db.Flights.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(f =>
            f.FlightNumber.ToLower().Contains(search.ToLower()) ||
            f.Departure.ToLower().Contains(search.ToLower()) ||
            f.Arrival.ToLower().Contains(search.ToLower()));
    }

    var flights = await query.ToListAsync();
    return Results.Ok(flights);
});

// Блок 3, уровень 3: список рейсов + фильтрация (query-параметр)
app.MapGet("/api/flights", async (AppDbContext db) =>
{
    var flights = await db.Flights.ToListAsync();
    return Results.Ok(flights);
});

// LINQ: фильтрация Рейсы дороже 100
app.MapGet("/api/flights/filter/expensive", async (AppDbContext db) =>
{
    var flights = await db.Flights
        .Where(f => f.Price > 100)
        .ToListAsync();

    return Results.Ok(flights);
});
// LINQ: сортировка Рейсы по возрастанию цены
app.MapGet("/api/flights/sorted/by-price", async (AppDbContext db) =>
{
    var flights = await db.Flights
        .OrderBy(f => f.Price)
        .ToListAsync();

    return Results.Ok(flights);
});
// LINQ: выборка с условием Только доступные рейсы, отсортированные по номеру
app.MapGet("/api/flights/available", async (AppDbContext db) =>
{
    var flights = await db.Flights
        .Where(f => f.IsAvailable)
        .OrderBy(f => f.FlightNumber)
        .ToListAsync();

    return Results.Ok(flights);
});
// LINQ: собственный запрос Рейсы из указанного города вылета
app.MapGet("/api/flights/from/{city}", async (string city, AppDbContext db) =>
{
    var flights = await db.Flights
        .Where(f => f.Departure.ToLower() == city.ToLower())
        .OrderBy(f => f.Price)
        .ToListAsync();

    return Results.Ok(flights);
});

// Блок 4: POST /api/flights
app.MapPost("/api/flights", async (Flight flight, AppDbContext db) =>
{
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

    db.Flights.Add(flight);
    await db.SaveChangesAsync();

    return Results.Created($"/api/flights/{flight.Id}", flight);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Блок 5, уровень 1: PUT /api/flights/{id}
app.MapPut("/api/flights/{id}", async (int id, Flight updated, AppDbContext db) =>
{
    var flight = await db.Flights.FirstOrDefaultAsync(f => f.Id == id);
    if (flight == null)
        return Results.NotFound();

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

    await db.SaveChangesAsync();

    return Results.Ok(flight);
});

// Блок 5, уровень 2: DELETE /api/flights/{id}
app.MapDelete("/api/flights/{id}", async (int id, AppDbContext db) =>
{
    var flight = await db.Flights.FirstOrDefaultAsync(f => f.Id == id);
    if (flight == null)
        return Results.NotFound();

    db.Flights.Remove(flight);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");
//app.Run();