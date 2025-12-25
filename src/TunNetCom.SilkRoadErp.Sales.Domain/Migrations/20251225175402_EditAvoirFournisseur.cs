using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class EditAvoirFournisseur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_Num_FactureFournisseur",
                table: "FactureAvoirFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_FactureAvoirFournisseur_Num",
                table: "FactureAvoirFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_AvoirFournisseur_Num",
                table: "AvoirFournisseur");

            migrationBuilder.DropColumn(
                name: "Num",
                table: "FactureAvoirFournisseur");

            migrationBuilder.DropColumn(
                name: "Num",
                table: "AvoirFournisseur");

            migrationBuilder.RenameColumn(
                name: "Num_FactureFournisseur",
                table: "FactureAvoirFournisseur",
                newName: "FactureFournisseurId");

            migrationBuilder.RenameIndex(
                name: "IX_FactureAvoirFournisseur_Num_FactureFournisseur",
                table: "FactureAvoirFournisseur",
                newName: "IX_FactureAvoirFournisseur_FactureFournisseurId");

            migrationBuilder.RenameColumn(
                name: "Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur",
                newName: "FactureAvoirFournisseurId");

            migrationBuilder.RenameColumn(
                name: "Num_AvoirFournisseur",
                table: "AvoirFournisseur",
                newName: "NumAvoirChezFournisseur");

            migrationBuilder.RenameIndex(
                name: "IX_AvoirFournisseur_Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur",
                newName: "IX_AvoirFournisseur_FactureAvoirFournisseurId");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_FactureAvoirFournisseurId",
                table: "AvoirFournisseur",
                column: "FactureAvoirFournisseurId",
                principalTable: "FactureAvoirFournisseur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_FactureFournisseurId",
                table: "FactureAvoirFournisseur",
                column: "FactureFournisseurId",
                principalTable: "FactureFournisseur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_FactureAvoirFournisseurId",
                table: "AvoirFournisseur");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_FactureFournisseurId",
                table: "FactureAvoirFournisseur");

            migrationBuilder.RenameColumn(
                name: "FactureFournisseurId",
                table: "FactureAvoirFournisseur",
                newName: "Num_FactureFournisseur");

            migrationBuilder.RenameIndex(
                name: "IX_FactureAvoirFournisseur_FactureFournisseurId",
                table: "FactureAvoirFournisseur",
                newName: "IX_FactureAvoirFournisseur_Num_FactureFournisseur");

            migrationBuilder.RenameColumn(
                name: "FactureAvoirFournisseurId",
                table: "AvoirFournisseur",
                newName: "Num_FactureAvoirFournisseur");

            migrationBuilder.RenameColumn(
                name: "NumAvoirChezFournisseur",
                table: "AvoirFournisseur",
                newName: "Num_AvoirFournisseur");

            migrationBuilder.RenameIndex(
                name: "IX_AvoirFournisseur_FactureAvoirFournisseurId",
                table: "AvoirFournisseur",
                newName: "IX_AvoirFournisseur_Num_FactureAvoirFournisseur");

            migrationBuilder.AddColumn<int>(
                name: "Num",
                table: "FactureAvoirFournisseur",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Num",
                table: "AvoirFournisseur",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_Num",
                table: "FactureAvoirFournisseur",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_Num",
                table: "AvoirFournisseur",
                column: "Num",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur",
                column: "Num_FactureAvoirFournisseur",
                principalTable: "FactureAvoirFournisseur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_Num_FactureFournisseur",
                table: "FactureAvoirFournisseur",
                column: "Num_FactureFournisseur",
                principalTable: "FactureFournisseur",
                principalColumn: "Num",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
