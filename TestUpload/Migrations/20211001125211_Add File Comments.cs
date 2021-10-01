using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class AddFileComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "fileUploads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "fileStorage",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "fileStorage");
        }
    }
}
