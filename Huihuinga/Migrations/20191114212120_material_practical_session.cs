using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class material_practical_session : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Material",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "MaterialName",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Material",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MaterialName",
                table: "Events");
        }
    }
}
