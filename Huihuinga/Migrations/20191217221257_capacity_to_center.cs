using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class capacity_to_center : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConferenceFeedbacks_Conferences_ConferenceId",
                table: "ConferenceFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ConferenceFeedbacks_ConferenceId",
                table: "ConferenceFeedbacks");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "EventCenters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "capacity",
                table: "EventCenters");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceFeedbacks_ConferenceId",
                table: "ConferenceFeedbacks",
                column: "ConferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConferenceFeedbacks_Conferences_ConferenceId",
                table: "ConferenceFeedbacks",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
