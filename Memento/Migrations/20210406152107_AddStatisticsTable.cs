using Microsoft.EntityFrameworkCore.Migrations;

namespace Memento.Migrations
{
    public partial class AddStatisticsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Statistics",
                table: "Statistics");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Statistics",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statistics",
                table: "Statistics",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_UserId",
                table: "Statistics",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Statistics",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_UserId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Statistics");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Statistics",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statistics",
                table: "Statistics",
                column: "UserId");
        }
    }
}
