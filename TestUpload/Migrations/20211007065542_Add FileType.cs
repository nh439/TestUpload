using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class AddFileType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "fileUploads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "fileStorage",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "fileStorage");
        }
    }
}
