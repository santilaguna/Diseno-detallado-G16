using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class material_to_talk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Talkid",
                table: "Materials",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Talkid",
                table: "Materials",
                column: "Talkid");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Events_Talkid",
                table: "Materials",
                column: "Talkid",
                principalTable: "Events",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Events_Talkid",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_Talkid",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Talkid",
                table: "Materials");
        }
    }
}
