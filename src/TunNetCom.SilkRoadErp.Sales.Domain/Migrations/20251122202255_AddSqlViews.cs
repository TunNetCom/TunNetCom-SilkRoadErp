using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSqlViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop ReceiptNoteView if it exists
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.ReceiptNoteView', 'V') IS NOT NULL
    DROP VIEW [dbo].[ReceiptNoteView];
");

            // Create ReceiptNoteView
            migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ReceiptNoteView]
AS
SELECT dbo.BonDeReception.Num, dbo.BonDeReception.date AS Date, SUM(dbo.LigneBonReception.tot_TTC) AS TotalTTC, dbo.BonDeReception.Num_Bon_fournisseur AS NumBonFournisseur, 
                  dbo.BonDeReception.date_livraison AS DateLivraison, dbo.BonDeReception.id_fournisseur AS IdFournisseur, dbo.BonDeReception.Num_Facture_fournisseur AS NumFactureFournisseur, SUM(dbo.LigneBonReception.tot_HT) 
                  AS TotHt
FROM     dbo.BonDeReception INNER JOIN
                  dbo.LigneBonReception ON dbo.BonDeReception.Num = dbo.LigneBonReception.Num_BonRec
GROUP BY dbo.BonDeReception.Num, dbo.BonDeReception.date, dbo.BonDeReception.Num_Bon_fournisseur, dbo.BonDeReception.date_livraison, dbo.BonDeReception.id_fournisseur, dbo.BonDeReception.Num_Facture_fournisseur;
");

            // Drop ProviderInvoiceView if it exists
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.ProviderInvoiceView', 'V') IS NOT NULL
    DROP VIEW [dbo].[ProviderInvoiceView];
");

            // Create ProviderInvoiceView
            migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ProviderInvoiceView]
AS
SELECT dbo.FactureFournisseur.Num, dbo.FactureFournisseur.id_fournisseur AS ProviderId, dbo.FactureFournisseur.NumFactureFournisseur AS ProviderInvoiceNumber, dbo.FactureFournisseur.dateFacturationFournisseur AS InvoicingDate, 
                  dbo.FactureFournisseur.date AS Date, SUM(dbo.LigneBonReception.tot_HT) AS TotalHT, SUM(dbo.LigneBonReception.tot_TTC) AS TotalTTC
FROM     dbo.FactureFournisseur INNER JOIN
                  dbo.BonDeReception ON dbo.FactureFournisseur.Num = dbo.BonDeReception.Num_Facture_fournisseur INNER JOIN
                  dbo.LigneBonReception ON dbo.BonDeReception.Num = dbo.LigneBonReception.Num_BonRec
GROUP BY dbo.FactureFournisseur.Num, dbo.FactureFournisseur.id_fournisseur, dbo.FactureFournisseur.NumFactureFournisseur, dbo.FactureFournisseur.dateFacturationFournisseur, dbo.FactureFournisseur.date;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[ReceiptNoteView]");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[ProviderInvoiceView]");
        }
    }
}
