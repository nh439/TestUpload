using Microsoft.EntityFrameworkCore.Migrations;

namespace TestUpload.Migrations
{
    public partial class RemoveErrorlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_errorLog_ErrorLogReference",
                table: "History");

            migrationBuilder.DropTable(
                name: "errorLog");

            migrationBuilder.DropIndex(
                name: "IX_History_ErrorLogReference",
                table: "History");

            migrationBuilder.DropColumn(
                name: "ErrorLogReference",
                table: "History");

            migrationBuilder.AddColumn<string>(
                name: "ErrorLog",
                table: "History",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorLog",
                table: "History");

            migrationBuilder.AddColumn<string>(
                name: "ErrorLogReference",
                table: "History",
                type: "varchar(767)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_History_ErrorLogReference",
                table: "History",
                column: "ErrorLogReference");

            migrationBuilder.AddForeignKey(
                name: "FK_History_errorLog_ErrorLogReference",
                table: "History",
                column: "ErrorLogReference",
                principalTable: "errorLog",
                principalColumn: "Reference",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
