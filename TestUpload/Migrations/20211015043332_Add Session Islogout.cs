using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class AddSessionIslogout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLogout",
                table: "sessions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLogout",
                table: "sessions");
        }
    }
}
