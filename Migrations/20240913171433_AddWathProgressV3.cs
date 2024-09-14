using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class AddWathProgressV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgresses_Episodes_AnimeId",
                table: "WatchProgresses");

            migrationBuilder.DropIndex(
                name: "IX_WatchProgresses_AnimeId",
                table: "WatchProgresses");

            migrationBuilder.DropColumn(
                name: "AnimeId",
                table: "WatchProgresses");

            migrationBuilder.RenameColumn(
                name: "EpisodeNumberId",
                table: "WatchProgresses",
                newName: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchProgresses_EpisodeId",
                table: "WatchProgresses",
                column: "EpisodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgresses_Episodes_EpisodeId",
                table: "WatchProgresses",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgresses_Episodes_EpisodeId",
                table: "WatchProgresses");

            migrationBuilder.DropIndex(
                name: "IX_WatchProgresses_EpisodeId",
                table: "WatchProgresses");

            migrationBuilder.RenameColumn(
                name: "EpisodeId",
                table: "WatchProgresses",
                newName: "EpisodeNumberId");

            migrationBuilder.AddColumn<int>(
                name: "AnimeId",
                table: "WatchProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WatchProgresses_AnimeId",
                table: "WatchProgresses",
                column: "AnimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgresses_Episodes_AnimeId",
                table: "WatchProgresses",
                column: "AnimeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
