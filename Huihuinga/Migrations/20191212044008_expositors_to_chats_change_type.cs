using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class expositors_to_chats_change_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "ExpositorsId",
                table: "Events",
                nullable: true,
                oldClrType: typeof(List<Guid>),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Guid>>(
                name: "ExpositorsId",
                table: "Events",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldNullable: true);
        }
    }
}
