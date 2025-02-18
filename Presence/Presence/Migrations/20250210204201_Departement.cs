using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presence.Migrations
{
    /// <inheritdoc />
    public partial class Departement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JourOut",
                table: "Presents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrganisation = table.Column<int>(type: "int", nullable: false),
                    NomDepartement = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departement_Organisations_IdOrganisation",
                        column: x => x.IdOrganisation,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departement_IdOrganisation",
                table: "Departement",
                column: "IdOrganisation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departement");

            migrationBuilder.DropColumn(
                name: "JourOut",
                table: "Presents");
        }
    }
}
