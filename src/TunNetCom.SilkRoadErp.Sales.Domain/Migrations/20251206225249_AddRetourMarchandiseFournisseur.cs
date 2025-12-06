using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRetourMarchandiseFournisseur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RetourMarchandiseFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    tot_H_tva = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tot_tva = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    net_payer = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.RetourMarchandiseFournisseur", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.RetourMarchandiseFournisseur_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.RetourMarchandiseFournisseur_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "LigneRetourMarchandiseFournisseur",
                columns: table => new
                {
                    Id_ligne = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetourMarchandiseFournisseurId = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneRetourMarchandiseFournisseur", x => x.Id_ligne);
                    table.ForeignKey(
                        name: "FK_dbo.LigneRetourMarchandiseFournisseur_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                    table.ForeignKey(
                        name: "FK_dbo.LigneRetourMarchandiseFournisseur_dbo.RetourMarchandiseFournisseur_RetourMarchandiseFournisseurId",
                        column: x => x.RetourMarchandiseFournisseurId,
                        principalTable: "RetourMarchandiseFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LigneRetourMarchandiseFournisseur_Ref_Produit",
                table: "LigneRetourMarchandiseFournisseur",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneRetourMarchandiseFournisseur_RetourMarchandiseFournisseurId",
                table: "LigneRetourMarchandiseFournisseur",
                column: "RetourMarchandiseFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_RetourMarchandiseFournisseur_AccountingYearId",
                table: "RetourMarchandiseFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_RetourMarchandiseFournisseur_id_fournisseur",
                table: "RetourMarchandiseFournisseur",
                column: "id_fournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_RetourMarchandiseFournisseur_Num",
                table: "RetourMarchandiseFournisseur",
                column: "Num",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LigneRetourMarchandiseFournisseur");

            migrationBuilder.DropTable(
                name: "RetourMarchandiseFournisseur");
        }
    }
}
