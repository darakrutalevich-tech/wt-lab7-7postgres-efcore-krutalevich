using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wt_lab7_7postgres_efcore_krutalevich.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Id", "Arrival", "CreatedAt", "Departure", "FlightNumber", "IsAvailable", "Price" },
                values: new object[,]
                {
                    { 1, "Варшава", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Минск", "AV-101", true, 250m },
                    { 2, "Берлин", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Минск", "AV-202", true, 320m },
                    { 3, "Москва", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Гомель", "AV-303", false, 180m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
