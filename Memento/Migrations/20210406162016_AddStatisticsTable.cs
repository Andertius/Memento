using Microsoft.EntityFrameworkCore.Migrations;

namespace Memento.Migrations
{
    public partial class AddStatisticsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HoursPerDay = table.Column<float>(type: "real", nullable: false),
                    CardsPerDay = table.Column<int>(type: "int", nullable: false),
                    AverageHoursPerDay = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_UserId",
                table: "Statistics",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistics");
        }
    }
}
