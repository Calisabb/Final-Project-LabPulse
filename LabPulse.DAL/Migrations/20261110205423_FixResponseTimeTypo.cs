using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabPulse.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixResponseTimeTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastResponceTimeMs",
                table: "Services",
                newName: "LastResponseTimeMs");

            migrationBuilder.RenameColumn(
                name: "ResponceTimeMs",
                table: "HealthLogs",
                newName: "ResponseTimeMs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckedAt",
                table: "HealthLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastResponseTimeMs",
                table: "Services",
                newName: "LastResponceTimeMs");

            migrationBuilder.RenameColumn(
                name: "ResponseTimeMs",
                table: "HealthLogs",
                newName: "ResponceTimeMs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckedAt",
                table: "HealthLogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
