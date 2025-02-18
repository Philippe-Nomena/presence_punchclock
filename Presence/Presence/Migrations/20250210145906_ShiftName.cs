using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presence.Migrations
{
    /// <inheritdoc />
    public partial class ShiftName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShiftName",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdShift",
                table: "Presents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Presents_IdShift",
                table: "Presents",
                column: "IdShift");

            migrationBuilder.AddForeignKey(
                name: "FK_Presents_Shifts_IdShift",
                table: "Presents",
                column: "IdShift",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presents_Shifts_IdShift",
                table: "Presents");

            migrationBuilder.DropIndex(
                name: "IX_Presents_IdShift",
                table: "Presents");

            migrationBuilder.DropColumn(
                name: "ShiftName",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IdShift",
                table: "Presents");
        }
    }
}
