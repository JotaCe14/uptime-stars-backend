using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptime.Stars.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingNextChecInMinutestotrackdownevents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NextCheckInMinutes",
                table: "Event",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextCheckInMinutes",
                table: "Event");
        }
    }
}
