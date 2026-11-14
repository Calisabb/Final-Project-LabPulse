using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabPulse.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, "#3B82F6", new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Dev Environment" },
                    { 2, "#10B981", new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Databases" },
                    { 3, "#F59E0B", new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Media" },
                    { 4, "#8B5CF6", new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Network Gateways" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
