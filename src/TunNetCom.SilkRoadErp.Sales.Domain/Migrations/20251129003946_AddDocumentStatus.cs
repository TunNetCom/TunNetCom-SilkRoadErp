using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "FactureFournisseur",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Facture",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Devis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Commandes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "BonDeReception",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "BonDeLivraison",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Avoirs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "AvoirFournisseur",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Statut",
                table: "FactureFournisseur");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Devis");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Commandes");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "BonDeReception");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "BonDeLivraison");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Avoirs");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "AvoirFournisseur");
        }
    }
}
