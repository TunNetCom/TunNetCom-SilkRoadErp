using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient",
                column: "BonDeLivraisonId",
                principalTable: "BonDeLivraison",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient",
                column: "FactureId",
                principalTable: "Facture",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur",
                column: "BonDeReceptionId",
                principalTable: "BonDeReception",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur",
                column: "FactureFournisseurId",
                principalTable: "FactureFournisseur",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient",
                column: "BonDeLivraisonId",
                principalTable: "BonDeLivraison",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient",
                column: "FactureId",
                principalTable: "Facture",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur",
                column: "BonDeReceptionId",
                principalTable: "BonDeReception",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur",
                column: "FactureFournisseurId",
                principalTable: "FactureFournisseur",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
