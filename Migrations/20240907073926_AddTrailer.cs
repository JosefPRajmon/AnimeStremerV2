using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class AddTrailer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Trailer",
                table: "Episodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trailer",
                table: "Episodes");
        }
    }
}
