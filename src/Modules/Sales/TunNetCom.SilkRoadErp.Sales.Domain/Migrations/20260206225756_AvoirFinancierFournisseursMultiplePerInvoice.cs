using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AvoirFinancierFournisseursMultiplePerInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_Num",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbo.AvoirFinancierFournisseurs",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AvoirFinancierFournisseurs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "NumFactureFournisseur",
                table: "AvoirFinancierFournisseurs",
                type: "int",
                nullable: true);

            // Preserve existing link: each row had Num = FactureFournisseur.Num (1:1)
            migrationBuilder.Sql("UPDATE AvoirFinancierFournisseurs SET NumFactureFournisseur = Num");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbo.AvoirFinancierFournisseurs",
                table: "AvoirFinancierFournisseurs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFinancierFournisseurs_Num",
                table: "AvoirFinancierFournisseurs",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFinancierFournisseurs_NumFactureFournisseur",
                table: "AvoirFinancierFournisseurs",
                column: "NumFactureFournisseur");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_NumFactureFournisseur",
                table: "AvoirFinancierFournisseurs",
                column: "NumFactureFournisseur",
                principalTable: "FactureFournisseur",
                principalColumn: "Num",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_NumFactureFournisseur",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbo.AvoirFinancierFournisseurs",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropIndex(
                name: "IX_AvoirFinancierFournisseurs_Num",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropIndex(
                name: "IX_AvoirFinancierFournisseurs_NumFactureFournisseur",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.DropColumn(
                name: "NumFactureFournisseur",
                table: "AvoirFinancierFournisseurs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbo.AvoirFinancierFournisseurs",
                table: "AvoirFinancierFournisseurs",
                column: "Num");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_Num",
                table: "AvoirFinancierFournisseurs",
                column: "Num",
                principalTable: "FactureFournisseur",
                principalColumn: "Num",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
