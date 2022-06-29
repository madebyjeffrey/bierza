using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bierza.Data.Migrations
{
    public partial class UpdateModelWithEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "UserNameUnique",
                table: "Users",
                newName: "EmailUnique");

            migrationBuilder.AddColumn<bool>(
                name: "Activated",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ValidatedEmail",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "DisplayNameUnique",
                table: "Users",
                column: "DisplayName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "DisplayNameUnique",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Activated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ValidatedEmail",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "EmailUnique",
                table: "Users",
                newName: "UserNameUnique");
        }
    }
}
