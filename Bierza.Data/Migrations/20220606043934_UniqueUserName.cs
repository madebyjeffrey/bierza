using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bierza.Data.Migrations
{
    public partial class UniqueUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UserNameUnique",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameUnique",
                table: "Users");
        }
    }
}
