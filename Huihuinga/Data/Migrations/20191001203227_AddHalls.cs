using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Data.Migrations
{
    public partial class AddHalls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    capacity = table.Column<int>(nullable: false),
                    location = table.Column<string>(nullable: true),
                    projector = table.Column<bool>(nullable: false),
                    plugs = table.Column<int>(nullable: false),
                    computers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Halls");
        }
    }
}
