using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class conferencefeedbacks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConferenceFeedbacks",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ConferenceId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_ConferenceFeedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_ConferenceFeedbacks_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceFeedbacks_ConferenceId",
                table: "ConferenceFeedbacks",
                column: "ConferenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConferenceFeedbacks");
        }
    }
}
