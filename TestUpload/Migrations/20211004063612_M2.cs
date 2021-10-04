using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class M2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rules",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Admin",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Rules",
                table: "User",
                type: "text",
                nullable: true);
        }
    }
}
