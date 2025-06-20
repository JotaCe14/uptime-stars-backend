using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptime.Stars.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentMonitor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Target = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IntervalInMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    TiemoutInMilliseconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 10000),
                    RequestHeaders = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SearchMode = table.Column<int>(type: "integer", nullable: true),
                    ExpectedText = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AlertEmails = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AlertMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AlertDelayMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    AlertResendCycles = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 6, 19, 22, 56, 7, 394, DateTimeKind.Utc).AddTicks(6011)),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentMonitor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentMonitor_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsUp = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LatencyMilliseconds = table.Column<long>(type: "bigint", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 6, 19, 22, 56, 7, 273, DateTimeKind.Utc).AddTicks(2401)),
                    Message = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FalsePositive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TicketId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MaintenanceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_ComponentMonitor_MonitorId",
                        column: x => x.MonitorId,
                        principalTable: "ComponentMonitor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentMonitor_GroupId",
                table: "ComponentMonitor",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_MonitorId",
                table: "Event",
                column: "MonitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "ComponentMonitor");

            migrationBuilder.DropTable(
                name: "Group");
        }
    }
}
