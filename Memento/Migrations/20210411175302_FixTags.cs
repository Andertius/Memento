using Microsoft.EntityFrameworkCore.Migrations;

namespace Memento.Migrations
{
    public partial class FixTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCards",
                table: "DeckTags");

            migrationBuilder.DropColumn(
                name: "TotalCards",
                table: "CardTags");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCards",
                table: "DeckTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCards",
                table: "CardTags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
