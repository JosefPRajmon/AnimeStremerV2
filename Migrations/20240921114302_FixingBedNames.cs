using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class FixingBedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WatchProgres",
                table: "Episodes",
                newName: "WatchProgress");

            migrationBuilder.RenameColumn(
                name: "EpisodaIcon",
                table: "Episodes",
                newName: "EpisodeIcon");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WatchProgress",
                table: "Episodes",
                newName: "WatchProgres");

            migrationBuilder.RenameColumn(
                name: "EpisodeIcon",
                table: "Episodes",
                newName: "EpisodaIcon");
        }
    }
}
