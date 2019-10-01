using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Data.Migrations
{
    public partial class AddRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "location",
                table: "Halls",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventCenterid",
                table: "Halls",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Halls_EventCenterid",
                table: "Halls",
                column: "EventCenterid");

            migrationBuilder.AddForeignKey(
                name: "FK_Halls_EventCenters_EventCenterid",
                table: "Halls",
                column: "EventCenterid",
                principalTable: "EventCenters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Halls_EventCenters_EventCenterid",
                table: "Halls");

            migrationBuilder.DropIndex(
                name: "IX_Halls_EventCenterid",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "EventCenterid",
                table: "Halls");

            migrationBuilder.AlterColumn<string>(
                name: "location",
                table: "Halls",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
