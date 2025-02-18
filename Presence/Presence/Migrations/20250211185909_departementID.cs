using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presence.Migrations
{
    /// <inheritdoc />
    public partial class departementID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departement_Organisations_IdOrganisation",
                table: "Departement");

            migrationBuilder.DropForeignKey(
                name: "FK_Presents_Organisations_IdOrganisation",
                table: "Presents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departement",
                table: "Departement");

            migrationBuilder.RenameTable(
                name: "Departement",
                newName: "Departements");

            migrationBuilder.RenameColumn(
                name: "IdOrganisation",
                table: "Presents",
                newName: "IdDepartement");

            migrationBuilder.RenameIndex(
                name: "IX_Presents_IdOrganisation",
                table: "Presents",
                newName: "IX_Presents_IdDepartement");

            migrationBuilder.RenameIndex(
                name: "IX_Departement_IdOrganisation",
                table: "Departements",
                newName: "IX_Departements_IdOrganisation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departements",
                table: "Departements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departements_Organisations_IdOrganisation",
                table: "Departements",
                column: "IdOrganisation",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presents_Departements_IdDepartement",
                table: "Presents",
                column: "IdDepartement",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departements_Organisations_IdOrganisation",
                table: "Departements");

            migrationBuilder.DropForeignKey(
                name: "FK_Presents_Departements_IdDepartement",
                table: "Presents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departements",
                table: "Departements");

            migrationBuilder.RenameTable(
                name: "Departements",
                newName: "Departement");

            migrationBuilder.RenameColumn(
                name: "IdDepartement",
                table: "Presents",
                newName: "IdOrganisation");

            migrationBuilder.RenameIndex(
                name: "IX_Presents_IdDepartement",
                table: "Presents",
                newName: "IX_Presents_IdOrganisation");

            migrationBuilder.RenameIndex(
                name: "IX_Departements_IdOrganisation",
                table: "Departement",
                newName: "IX_Departement_IdOrganisation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departement",
                table: "Departement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departement_Organisations_IdOrganisation",
                table: "Departement",
                column: "IdOrganisation",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presents_Organisations_IdOrganisation",
                table: "Presents",
                column: "IdOrganisation",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
