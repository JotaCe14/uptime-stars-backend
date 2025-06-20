using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptime.Stars.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsImportantproperytoEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Event",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Event");
        }
    }
}
