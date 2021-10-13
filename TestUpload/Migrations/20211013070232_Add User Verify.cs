using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class AddUserVerify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerifyBy",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifyDate",
                table: "User",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Suspend",
                table: "login",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "login",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Verify",
                table: "login",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifyBy",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VerifyDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Suspend",
                table: "login");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "login");

            migrationBuilder.DropColumn(
                name: "Verify",
                table: "login");
        }
    }
}
