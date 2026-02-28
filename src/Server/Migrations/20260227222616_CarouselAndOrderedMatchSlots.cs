using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartPerformanceTracker.Server.Migrations
{
    /// <inheritdoc />
    public partial class CarouselAndOrderedMatchSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfMatches",
                table: "SeasonMatchConfigurations");

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "GameNights",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "MatchTypes",
                columns: new[] { "Id", "Name", "PlayersPerSide" },
                values: new object[] { 6, "Blind Pairs", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MatchTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "GameNights");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfMatches",
                table: "SeasonMatchConfigurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
