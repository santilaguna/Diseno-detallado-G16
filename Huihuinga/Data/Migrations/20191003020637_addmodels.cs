using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Data.Migrations
{
    public partial class addmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Meal",
                table: "Meal");

            migrationBuilder.RenameTable(
                name: "Meal",
                newName: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Events",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "material",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Talk_description",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Talk_material",
                table: "Events",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "image",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "material",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Talk_description",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Talk_material",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Meal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meal",
                table: "Meal",
                column: "id");
        }
    }
}
