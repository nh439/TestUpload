using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class G : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "login",
                type: "varchar(514)",
                maxLength: 514,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "fileUploads",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "fileStorage",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "fileUploads");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "fileStorage");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "login",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(514)",
                oldMaxLength: 514);
        }
    }
}
