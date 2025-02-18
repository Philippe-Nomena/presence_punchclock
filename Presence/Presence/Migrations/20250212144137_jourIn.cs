using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presence.Migrations
{
    /// <inheritdoc />
    public partial class jourIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JourIn",
                table: "Presents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JourIn",
                table: "Presents");
        }
    }
}
