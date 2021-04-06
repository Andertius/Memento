using Microsoft.EntityFrameworkCore.Migrations;

namespace Memento.Migrations
{
    public partial class ChangeSettingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoursPerDay = table.Column<float>(type: "real", nullable: false),
                    CardsPerDay = table.Column<int>(type: "int", nullable: false),
                    CardsOrder = table.Column<int>(type: "int", nullable: false),
                    DarkTheme = table.Column<bool>(type: "bit", nullable: false),
                    ShowImages = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Settings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
