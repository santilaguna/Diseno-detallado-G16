using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class change_type_material : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Material",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MaterialName",
                table: "Events");

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    filename = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    EventId = table.Column<Guid>(nullable: false),
                    PracticalSessionid = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materials_Events_PracticalSessionid",
                        column: x => x.PracticalSessionid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_EventId",
                table: "Materials",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_PracticalSessionid",
                table: "Materials",
                column: "PracticalSessionid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.AddColumn<List<string>>(
                name: "Material",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "MaterialName",
                table: "Events",
                nullable: true);
        }
    }
}
