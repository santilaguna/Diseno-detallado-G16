using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class material_charla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Talk_material",
                table: "Events");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Talk_material",
                table: "Events",
                nullable: true);
        }
    }
}
