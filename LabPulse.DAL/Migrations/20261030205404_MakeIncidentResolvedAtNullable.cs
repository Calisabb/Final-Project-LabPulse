using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabPulse.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeIncidentResolvedAtNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidents_ServiceId",
                table: "Incidents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedAt",
                table: "Incidents",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidents_ServiceId",
                table: "Incidents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedAt",
                table: "Incidents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
