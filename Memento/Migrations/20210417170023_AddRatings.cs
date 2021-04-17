using Microsoft.EntityFrameworkCore.Migrations;

namespace Memento.Migrations
{
    public partial class AddRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Decks");

            migrationBuilder.CreateTable(
                name: "UserDeckRating",
                columns: table => new
                {
                    DeckId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeckRating", x => new { x.DeckId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserDeckRating_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDeckRating_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDeckRating_UserId",
                table: "UserDeckRating",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDeckRating");

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Decks",
                type: "decimal(2,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
