using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderInvoiceViewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
                migrationBuilder.Sql(@"
            CREATE VIEW [dbo].[ProviderInvoiceView]
AS
SELECT dbo.FactureFournisseur.Num, dbo.FactureFournisseur.id_fournisseur AS ProviderId, dbo.FactureFournisseur.NumFactureFournisseur AS ProviderInvoiceNumber, dbo.FactureFournisseur.dateFacturationFournisseur AS InvoicingDate, 
                  dbo.FactureFournisseur.date AS Date, SUM(dbo.LigneBonReception.tot_HT) AS TotalHT, SUM(dbo.LigneBonReception.tot_TTC) AS TotalTTC
FROM     dbo.FactureFournisseur INNER JOIN
                  dbo.BonDeReception ON dbo.FactureFournisseur.Num = dbo.BonDeReception.Num_Facture_fournisseur INNER JOIN
                  dbo.LigneBonReception ON dbo.BonDeReception.Num = dbo.LigneBonReception.Num_BonRec
GROUP BY dbo.FactureFournisseur.Num, dbo.FactureFournisseur.id_fournisseur, dbo.FactureFournisseur.NumFactureFournisseur, dbo.FactureFournisseur.dateFacturationFournisseur, dbo.FactureFournisseur.date

        ");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[ProviderInvoiceView]");
        }
    }
}
