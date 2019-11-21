using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class menu_to_meals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    filename = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    menu = table.Column<string>(nullable: true),
                    EventId = table.Column<Guid>(nullable: false),
                    Mealid = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Menus_Events_Mealid",
                        column: x => x.Mealid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_EventId",
                table: "Menus",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Mealid",
                table: "Menus",
                column: "Mealid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
