using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meets.WebApi.Migrations
{
    public partial class AddSigningUpForMeetups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "meetups_signed_up_users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meetup_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meetups_signed_up_users", x => new { x.user_id, x.meetup_id });
                    table.ForeignKey(
                        name: "fk_meetups_signed_up_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_signed_up_meetups",
                        column: x => x.meetup_id,
                        principalTable: "meetups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_meetups_signed_up_users_meetup_id",
                table: "meetups_signed_up_users",
                column: "meetup_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meetups_signed_up_users");
        }
    }
}
