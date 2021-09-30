using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace TestUpload.Migrations
{
    public partial class C : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fileStorage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(767)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Filename = table.Column<string>(type: "text", nullable: true),
                    FileExtension = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RawData = table.Column<byte[]>(type: "varbinary(4000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fileStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fileUploads",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(767)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Filename = table.Column<string>(type: "text", nullable: true),
                    FileExtension = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fileUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "login",
                columns: table => new
                {
                    Username = table.Column<string>(type: "varchar(767)", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Firstname = table.Column<string>(type: "text", nullable: true),
                    Lastname = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "varchar(767)", nullable: false),
                    Registerd = table.Column<DateTime>(type: "datetime", nullable: false),
                    Rules = table.Column<string>(type: "text", nullable: true),
                    LoginUsername = table.Column<string>(type: "varchar(767)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_login_LoginUsername",
                        column: x => x.LoginUsername,
                        principalTable: "login",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_LoginUsername",
                table: "User",
                column: "LoginUsername");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fileStorage");

            migrationBuilder.DropTable(
                name: "fileUploads");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "login");
        }
    }
}
