using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptNoteViewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [dbo].[ReceiptNoteView]
AS
SELECT dbo.BonDeReception.Num, dbo.BonDeReception.date AS Date, SUM(dbo.LigneBonReception.tot_TTC) AS TotalTTC, dbo.BonDeReception.Num_Bon_fournisseur AS NumBonFournisseur, 
                  dbo.BonDeReception.date_livraison AS DateLivraison, dbo.BonDeReception.id_fournisseur AS IdFournisseur, dbo.BonDeReception.Num_Facture_fournisseur AS NumFactureFournisseur, SUM(dbo.LigneBonReception.tot_HT) 
                  AS TotHt
FROM     dbo.BonDeReception INNER JOIN
                  dbo.LigneBonReception ON dbo.BonDeReception.Num = dbo.LigneBonReception.Num_BonRec
GROUP BY dbo.BonDeReception.Num, dbo.BonDeReception.date, dbo.BonDeReception.Num_Bon_fournisseur, dbo.BonDeReception.date_livraison, dbo.BonDeReception.id_fournisseur, dbo.BonDeReception.Num_Facture_fournisseur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[ReceiptNoteView]");
        }
    }
}
