using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeForeignKeysToUseIdInsteadOfNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.LigneBL_dbo.BonDeLivraison_Num_BL",
                table: "LigneBL");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.LigneBonReception_dbo.BonDeReception_Num_BonRec",
                table: "LigneBonReception");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Transaction_dbo.BonDeLivraison_Num_BL",
                table: "Transaction");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_BonDeReception_Num",
                table: "BonDeReception");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_BonDeLivraison_Num",
                table: "BonDeLivraison");

            migrationBuilder.RenameColumn(
                name: "Num_BL",
                table: "Transaction",
                newName: "BonDeLivraisonId");

            migrationBuilder.RenameColumn(
                name: "Num_BonRec",
                table: "LigneBonReception",
                newName: "BonDeReceptionId");

            migrationBuilder.RenameIndex(
                name: "IX_LigneBonReception_Num_BonRec",
                table: "LigneBonReception",
                newName: "IX_LigneBonReception_BonDeReceptionId");

            migrationBuilder.RenameColumn(
                name: "Num_BL",
                table: "LigneBL",
                newName: "BonDeLivraisonId");

            migrationBuilder.RenameIndex(
                name: "IX_LigneBL_Num_BL",
                table: "LigneBL",
                newName: "IX_LigneBL_BonDeLivraisonId");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.LigneBL_dbo.BonDeLivraison_BonDeLivraisonId",
                table: "LigneBL",
                column: "BonDeLivraisonId",
                principalTable: "BonDeLivraison",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.LigneBonReception_dbo.BonDeReception_BonDeReceptionId",
                table: "LigneBonReception",
                column: "BonDeReceptionId",
                principalTable: "BonDeReception",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Transaction_dbo.BonDeLivraison_BonDeLivraisonId",
                table: "Transaction",
                column: "BonDeLivraisonId",
                principalTable: "BonDeLivraison",
                principalColumn: "Id");

            // Re-add unique constraints on Num (they are still needed for uniqueness, just not for foreign keys)
            migrationBuilder.AddUniqueConstraint(
                name: "AK_BonDeReception_Num",
                table: "BonDeReception",
                column: "Num");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BonDeLivraison_Num",
                table: "BonDeLivraison",
                column: "Num");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.LigneBL_dbo.BonDeLivraison_BonDeLivraisonId",
                table: "LigneBL");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.LigneBonReception_dbo.BonDeReception_BonDeReceptionId",
                table: "LigneBonReception");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Transaction_dbo.BonDeLivraison_BonDeLivraisonId",
                table: "Transaction");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_BonDeReception_Num",
                table: "BonDeReception");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_BonDeLivraison_Num",
                table: "BonDeLivraison");

            migrationBuilder.RenameColumn(
                name: "BonDeLivraisonId",
                table: "Transaction",
                newName: "Num_BL");

            migrationBuilder.RenameColumn(
                name: "BonDeReceptionId",
                table: "LigneBonReception",
                newName: "Num_BonRec");

            migrationBuilder.RenameIndex(
                name: "IX_LigneBonReception_BonDeReceptionId",
                table: "LigneBonReception",
                newName: "IX_LigneBonReception_Num_BonRec");

            migrationBuilder.RenameColumn(
                name: "BonDeLivraisonId",
                table: "LigneBL",
                newName: "Num_BL");

            migrationBuilder.RenameIndex(
                name: "IX_LigneBL_BonDeLivraisonId",
                table: "LigneBL",
                newName: "IX_LigneBL_Num_BL");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BonDeReception_Num",
                table: "BonDeReception",
                column: "Num");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BonDeLivraison_Num",
                table: "BonDeLivraison",
                column: "Num");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.LigneBL_dbo.BonDeLivraison_Num_BL",
                table: "LigneBL",
                column: "Num_BL",
                principalTable: "BonDeLivraison",
                principalColumn: "Num",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.LigneBonReception_dbo.BonDeReception_Num_BonRec",
                table: "LigneBonReception",
                column: "Num_BonRec",
                principalTable: "BonDeReception",
                principalColumn: "Num",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Transaction_dbo.BonDeLivraison_Num_BL",
                table: "Transaction",
                column: "Num_BL",
                principalTable: "BonDeLivraison",
                principalColumn: "Num");
        }
    }
}
