using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLigneDevisForeignKeyToUseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.LigneDevis_dbo.Devis_Num_devis",
                table: "LigneDevis");

            migrationBuilder.RenameColumn(
                name: "Num_devis",
                table: "LigneDevis",
                newName: "DevisId");

            migrationBuilder.RenameIndex(
                name: "IX_LigneDevis_Num_devis",
                table: "LigneDevis",
                newName: "IX_LigneDevis_DevisId");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.LigneDevis_dbo.Devis_DevisId",
                table: "LigneDevis",
                column: "DevisId",
                principalTable: "Devis",
                principalColumn: "Num",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.LigneDevis_dbo.Devis_DevisId",
                table: "LigneDevis");

            migrationBuilder.RenameColumn(
                name: "DevisId",
                table: "LigneDevis",
                newName: "Num_devis");

            migrationBuilder.RenameIndex(
                name: "IX_LigneDevis_DevisId",
                table: "LigneDevis",
                newName: "IX_LigneDevis_Num_devis");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.LigneDevis_dbo.Devis_Num_devis",
                table: "LigneDevis",
                column: "Num_devis",
                principalTable: "Devis",
                principalColumn: "Num");
        }
    }
}
