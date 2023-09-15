using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventRegistrationApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalSeats = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Date", "Name", "TotalSeats" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 9, 22, 11, 54, 20, 295, DateTimeKind.Local).AddTicks(970), "Event 1", 60 },
                    { 2, new DateTime(2023, 9, 29, 11, 54, 20, 295, DateTimeKind.Local).AddTicks(995), "Event 2", 70 },
                    { 3, new DateTime(2023, 10, 6, 11, 54, 20, 295, DateTimeKind.Local).AddTicks(997), "Event 3", 80 },
                    { 4, new DateTime(2023, 10, 13, 11, 54, 20, 295, DateTimeKind.Local).AddTicks(998), "Event 4", 90 },
                    { 5, new DateTime(2023, 10, 20, 11, 54, 20, 295, DateTimeKind.Local).AddTicks(1000), "Event 5", 100 }
                });

            migrationBuilder.InsertData(
                table: "Registrations",
                columns: new[] { "Id", "EventId", "ReferenceNumber", "UserId" },
                values: new object[,]
                {
                    { 1, 2, "Ref1", "User1" },
                    { 2, 3, "Ref2", "User2" },
                    { 3, 4, "Ref3", "User3" },
                    { 4, 5, "Ref4", "User4" },
                    { 5, 1, "Ref5", "User5" },
                    { 6, 2, "Ref6", "User6" },
                    { 7, 3, "Ref7", "User7" },
                    { 8, 4, "Ref8", "User8" },
                    { 9, 5, "Ref9", "User9" },
                    { 10, 1, "Ref10", "User10" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_EventId",
                table: "Registrations",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
