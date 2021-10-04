using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace TestUpload.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "changepassword",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    BySystem = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ResetPasswordRequire = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_changepassword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "errorLog",
                columns: table => new
                {
                    Reference = table.Column<string>(type: "varchar(767)", nullable: false),
                    ExceptionMessage = table.Column<string>(type: "text", nullable: true),
                    InnerException = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_errorLog", x => x.Reference);
                });

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
                    LastUpdate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RawData = table.Column<byte[]>(type: "longblob", nullable: true),
                    pass = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
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
                    AddDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime", nullable: false),
                    pass = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
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
                    Password = table.Column<string>(type: "varchar(514)", maxLength: 514, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(767)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoggedIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    Loggedout = table.Column<DateTime>(type: "datetime", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    HistoryMode = table.Column<string>(type: "text", nullable: true),
                    RelatedFile = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Issuccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErrorLogReference = table.Column<string>(type: "varchar(767)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_errorLog_ErrorLogReference",
                        column: x => x.ErrorLogReference,
                        principalTable: "errorLog",
                        principalColumn: "Reference",
                        onDelete: ReferentialAction.Restrict);
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
                    LoginUsername = table.Column<string>(type: "varchar(767)", nullable: true),
                    Male = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BrithDay = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "IX_History_ErrorLogReference",
                table: "History",
                column: "ErrorLogReference");

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
                name: "changepassword");

            migrationBuilder.DropTable(
                name: "fileStorage");

            migrationBuilder.DropTable(
                name: "fileUploads");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "errorLog");

            migrationBuilder.DropTable(
                name: "login");
        }
    }
}
