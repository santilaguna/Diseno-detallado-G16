using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Huihuinga.Migrations
{
    public partial class addEventTopics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Halls_Hallid",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Events_Chatid",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Events_PracticalSessionid",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Events_Talkid",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_Chatid",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_PracticalSessionid",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_Talkid",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Events_Hallid",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Chatid",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "PracticalSessionid",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Talkid",
                table: "Topics");

            migrationBuilder.CreateTable(
                name: "EventTopics",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    TopicId = table.Column<Guid>(nullable: false),
                    Chatid = table.Column<Guid>(nullable: true),
                    PracticalSessionid = table.Column<Guid>(nullable: true),
                    Talkid = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTopics", x => new { x.EventId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_EventTopics_Events_Chatid",
                        column: x => x.Chatid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTopics_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTopics_Events_PracticalSessionid",
                        column: x => x.PracticalSessionid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTopics_Events_Talkid",
                        column: x => x.Talkid,
                        principalTable: "Events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTopics_Chatid",
                table: "EventTopics",
                column: "Chatid");

            migrationBuilder.CreateIndex(
                name: "IX_EventTopics_PracticalSessionid",
                table: "EventTopics",
                column: "PracticalSessionid");

            migrationBuilder.CreateIndex(
                name: "IX_EventTopics_Talkid",
                table: "EventTopics",
                column: "Talkid");

            migrationBuilder.CreateIndex(
                name: "IX_EventTopics_TopicId",
                table: "EventTopics",
                column: "TopicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTopics");

            migrationBuilder.AddColumn<Guid>(
                name: "Chatid",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PracticalSessionid",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Talkid",
                table: "Topics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Chatid",
                table: "Topics",
                column: "Chatid");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_PracticalSessionid",
                table: "Topics",
                column: "PracticalSessionid");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Talkid",
                table: "Topics",
                column: "Talkid");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Hallid",
                table: "Events",
                column: "Hallid");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Halls_Hallid",
                table: "Events",
                column: "Hallid",
                principalTable: "Halls",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Events_Chatid",
                table: "Topics",
                column: "Chatid",
                principalTable: "Events",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Events_PracticalSessionid",
                table: "Topics",
                column: "PracticalSessionid",
                principalTable: "Events",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Events_Talkid",
                table: "Topics",
                column: "Talkid",
                principalTable: "Events",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
