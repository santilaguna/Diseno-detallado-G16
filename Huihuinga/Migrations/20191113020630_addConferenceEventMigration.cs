using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class addConferenceEventMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ConcreteConferences_ConcreteConferenceid",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "ConcreteConferenceid",
                table: "Events",
                newName: "concreteConferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_ConcreteConferenceid",
                table: "Events",
                newName: "IX_Events_concreteConferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ConcreteConferences_concreteConferenceId",
                table: "Events",
                column: "concreteConferenceId",
                principalTable: "ConcreteConferences",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ConcreteConferences_concreteConferenceId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "concreteConferenceId",
                table: "Events",
                newName: "ConcreteConferenceid");

            migrationBuilder.RenameIndex(
                name: "IX_Events_concreteConferenceId",
                table: "Events",
                newName: "IX_Events_ConcreteConferenceid");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ConcreteConferences_ConcreteConferenceid",
                table: "Events",
                column: "ConcreteConferenceid",
                principalTable: "ConcreteConferences",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
