using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Data.Migrations
{
    public partial class addEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meal",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    starttime = table.Column<DateTime>(nullable: false),
                    endtime = table.Column<DateTime>(nullable: false),
                    Hallid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meal", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meal");
        }
    }
}
