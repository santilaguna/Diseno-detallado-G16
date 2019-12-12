using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class owner_to_conferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Conferences",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ConcreteConferences",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Conferences");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ConcreteConferences");
        }
    }
}
