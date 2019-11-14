using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class HallEventRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Events_Hallid",
                table: "Events",
                column: "Hallid");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Halls_Hallid",
                table: "Events",
                column: "Hallid",
                principalTable: "Halls",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Halls_Hallid",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_Hallid",
                table: "Events");
        }
    }
}
