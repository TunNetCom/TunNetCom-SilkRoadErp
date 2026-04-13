using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "TiersDepenseFonctionnement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "TejCertificatSequence",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "TejCertificatFactureDepense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "TejCertificatFacture",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Tag",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Systeme",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SousFamilleProduit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExternalGroupId",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RolePermissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RetourMarchandiseFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RetenueSourceFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RetenueSourceFactureDepense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RetenueSourceClient",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReceptionRetourFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Produit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PrintHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementTiersDepenseFactureDepense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementTiersDepense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementFournisseurFactureFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementFournisseurBonDeReception",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementClientFacture",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementClientBonDeLivraison",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PaiementClient",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneRetourMarchandiseFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneInventaire",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneDevis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneCommandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneBonReception",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneBL",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneAvoirs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LigneAvoirFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Inventaire",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InstallationTechnician",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Fournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FamilleProduit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FactureFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FactureDepense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FactureAvoirFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FactureAvoirClient",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Facture",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EcheanceDesFournisseurs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "DocumentTag",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Devis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "DeliveryCar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CompteBancaire",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Commandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "BonDeReception",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "BonDeLivraison",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Banque",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "BankTransactionImport",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "BankTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Avoirs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AvoirFournisseur",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AvoirFinancierFournisseurs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AccountingYear",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TiersDepenseFonctionnement");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TejCertificatSequence");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TejCertificatFactureDepense");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TejCertificatFacture");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SousFamilleProduit");

            migrationBuilder.DropColumn(
                name: "ExternalGroupId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RetourMarchandiseFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RetenueSourceFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RetenueSourceFactureDepense");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RetenueSourceClient");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReceptionRetourFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Produit");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PrintHistory");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementTiersDepenseFactureDepense");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementTiersDepense");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementFournisseurFactureFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementFournisseurBonDeReception");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementClientFacture");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementClientBonDeLivraison");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaiementClient");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneRetourMarchandiseFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneInventaire");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneDevis");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneCommandes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneBonReception");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneBL");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneAvoirs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LigneAvoirFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Inventaire");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InstallationTechnician");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Fournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FamilleProduit");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FactureFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FactureAvoirFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FactureAvoirClient");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EcheanceDesFournisseurs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DocumentTag");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Devis");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DeliveryCar");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CompteBancaire");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Commandes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BonDeReception");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BonDeLivraison");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Banque");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BankTransactionImport");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BankTransaction");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Avoirs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AvoirFournisseur");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AccountingYear");
        }
    }
}
