using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Data.Migrations
{
    public partial class AddNameToHalls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Halls",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "Halls");
        }
    }
}
