using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class AddFileStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                table: "fileUploads",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "fileUploads",
                type: "varchar(767)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadId",
                table: "fileUploads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uploadname",
                table: "fileUploads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                table: "fileStorage",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "fileStorage",
                type: "varchar(767)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadId",
                table: "fileStorage",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uploadname",
                table: "fileStorage",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_fileUploads_Token",
                table: "fileUploads",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fileStorage_Token",
                table: "fileStorage",
                column: "Token",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_fileUploads_Token",
                table: "fileUploads");

            migrationBuilder.DropIndex(
                name: "IX_fileStorage_Token",
                table: "fileStorage");

            migrationBuilder.DropColumn(
                name: "Shared",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "Uploadname",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "Shared",
                table: "fileStorage");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "fileStorage");

            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "fileStorage");

            migrationBuilder.DropColumn(
                name: "Uploadname",
                table: "fileStorage");
        }
    }
}
