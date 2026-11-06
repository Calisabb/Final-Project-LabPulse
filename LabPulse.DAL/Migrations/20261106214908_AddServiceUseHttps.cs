using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabPulse.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceUseHttps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastCheked",
                table: "Services",
                newName: "LastCheckedAt");

            migrationBuilder.AddColumn<bool>(
                name: "UseHttps",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseHttps",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "LastCheckedAt",
                table: "Services",
                newName: "LastCheked");
        }
    }
}
