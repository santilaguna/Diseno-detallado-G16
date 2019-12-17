using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class add_feedbacks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    EventId = table.Column<Guid>(nullable: false),
                    dateTime = table.Column<DateTime>(nullable: false),
                    comment = table.Column<string>(nullable: true),
                    FoodQuality = table.Column<int>(nullable: false),
                    MusicQuality = table.Column<int>(nullable: false),
                    DiscussionQuality = table.Column<int>(nullable: false),
                    MaterialQuality = table.Column<int>(nullable: false),
                    PlaceQuality = table.Column<int>(nullable: false),
                    ExpositorQuality = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_EventId",
                table: "Feedbacks",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");
        }
    }
}
