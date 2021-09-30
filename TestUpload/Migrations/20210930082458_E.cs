using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class E : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pass",
                table: "fileUploads",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pass",
                table: "fileStorage",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pass",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "pass",
                table: "fileStorage");
        }
    }
}
