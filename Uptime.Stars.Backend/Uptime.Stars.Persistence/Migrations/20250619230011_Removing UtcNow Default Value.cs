using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptime.Stars.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovingUtcNowDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimestampUtc",
                table: "Event",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 19, 22, 56, 7, 273, DateTimeKind.Utc).AddTicks(2401));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ComponentMonitor",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 19, 22, 56, 7, 394, DateTimeKind.Utc).AddTicks(6011));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimestampUtc",
                table: "Event",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 19, 22, 56, 7, 273, DateTimeKind.Utc).AddTicks(2401),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ComponentMonitor",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 19, 22, 56, 7, 394, DateTimeKind.Utc).AddTicks(6011),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
