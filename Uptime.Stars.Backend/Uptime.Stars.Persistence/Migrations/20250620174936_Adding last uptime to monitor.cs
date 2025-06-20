using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptime.Stars.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addinglastuptimetomonitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUp",
                table: "ComponentMonitor",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUp",
                table: "ComponentMonitor");
        }
    }
}
