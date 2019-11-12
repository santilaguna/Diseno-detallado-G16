using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class addTopicsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    Chatid = table.Column<Guid>(nullable: true),
                    PracticalSessionid = table.Column<Guid>(nullable: true),
                    Talkid = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.id);
                    table.ForeignKey(
                        name: "FK_Topics_Events_Chatid",
                        column: x => x.Chatid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Events_PracticalSessionid",
                        column: x => x.PracticalSessionid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Events_Talkid",
                        column: x => x.Talkid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Chatid",
                table: "Topics",
                column: "Chatid");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_PracticalSessionid",
                table: "Topics",
                column: "PracticalSessionid");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Talkid",
                table: "Topics",
                column: "Talkid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Topics");
        }
    }
}
