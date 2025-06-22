using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptime.Stars.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingEnumstoMaintenanceTypeandCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceType",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceType",
                table: "Event",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceType",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceType",
                table: "Event",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
