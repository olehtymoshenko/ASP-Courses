using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meets.WebApi.Migrations
{
    public partial class FixMeetupNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Meetups",
                table: "Meetups");

            migrationBuilder.RenameTable(
                name: "Meetups",
                newName: "meetups");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "meetups",
                newName: "topic");

            migrationBuilder.RenameColumn(
                name: "Place",
                table: "meetups",
                newName: "place");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "meetups",
                newName: "duration");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "meetups",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_meetups",
                table: "meetups",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_meetups",
                table: "meetups");

            migrationBuilder.RenameTable(
                name: "meetups",
                newName: "Meetups");

            migrationBuilder.RenameColumn(
                name: "topic",
                table: "Meetups",
                newName: "Topic");

            migrationBuilder.RenameColumn(
                name: "place",
                table: "Meetups",
                newName: "Place");

            migrationBuilder.RenameColumn(
                name: "duration",
                table: "Meetups",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Meetups",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meetups",
                table: "Meetups",
                column: "Id");
        }
    }
}
