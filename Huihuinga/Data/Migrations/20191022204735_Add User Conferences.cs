using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class AddUserConferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserConcreteConference_ConcreteConferences_Confe~",
                table: "ApplicationUserConcreteConference");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserConcreteConference_AspNetUsers_UserId",
                table: "ApplicationUserConcreteConference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserConcreteConference",
                table: "ApplicationUserConcreteConference");

            migrationBuilder.RenameTable(
                name: "ApplicationUserConcreteConference",
                newName: "UserConferences");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserConcreteConference_ConferenceId",
                table: "UserConferences",
                newName: "IX_UserConferences_ConferenceId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConferences",
                table: "UserConferences",
                columns: new[] { "UserId", "ConferenceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserConferences_ConcreteConferences_ConferenceId",
                table: "UserConferences",
                column: "ConferenceId",
                principalTable: "ConcreteConferences",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConferences_AspNetUsers_UserId",
                table: "UserConferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConferences_ConcreteConferences_ConferenceId",
                table: "UserConferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConferences_AspNetUsers_UserId",
                table: "UserConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConferences",
                table: "UserConferences");

            migrationBuilder.RenameTable(
                name: "UserConferences",
                newName: "ApplicationUserConcreteConference");

            migrationBuilder.RenameIndex(
                name: "IX_UserConferences_ConferenceId",
                table: "ApplicationUserConcreteConference",
                newName: "IX_ApplicationUserConcreteConference_ConferenceId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserConcreteConference",
                table: "ApplicationUserConcreteConference",
                columns: new[] { "UserId", "ConferenceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserConcreteConference_ConcreteConferences_Confe~",
                table: "ApplicationUserConcreteConference",
                column: "ConferenceId",
                principalTable: "ConcreteConferences",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserConcreteConference_AspNetUsers_UserId",
                table: "ApplicationUserConcreteConference",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
