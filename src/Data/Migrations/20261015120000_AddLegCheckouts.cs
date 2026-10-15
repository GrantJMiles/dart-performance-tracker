using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartPerformanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLegCheckouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaximumCheckout",
                table: "Seasons",
                type: "int",
                nullable: false,
                defaultValue: 170);

            migrationBuilder.CreateTable(
                name: "LegCheckouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegCheckouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegCheckouts_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegCheckouts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegCheckouts_MatchId",
                table: "LegCheckouts",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_LegCheckouts_PlayerId",
                table: "LegCheckouts",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegCheckouts");

            migrationBuilder.DropColumn(
                name: "MaximumCheckout",
                table: "Seasons");
        }
    }
}
