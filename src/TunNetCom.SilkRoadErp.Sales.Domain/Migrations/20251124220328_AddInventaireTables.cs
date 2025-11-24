using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddInventaireTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    DateInventaire = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Inventaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Inventaire_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LigneInventaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventaireId = table.Column<int>(type: "int", nullable: false),
                    RefProduit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantiteTheorique = table.Column<int>(type: "int", nullable: false),
                    QuantiteReelle = table.Column<int>(type: "int", nullable: false),
                    PrixHt = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DernierPrixAchat = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ProduitRefe = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneInventaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LigneInventaire_Produit_ProduitRefe",
                        column: x => x.ProduitRefe,
                        principalTable: "Produit",
                        principalColumn: "refe");
                    table.ForeignKey(
                        name: "FK_dbo.LigneInventaire_dbo.Inventaire_InventaireId",
                        column: x => x.InventaireId,
                        principalTable: "Inventaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneInventaire_dbo.Produit_RefProduit",
                        column: x => x.RefProduit,
                        principalTable: "Produit",
                        principalColumn: "refe",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventaire_AccountingYearId",
                table: "Inventaire",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventaire_AccountingYearId_Num",
                table: "Inventaire",
                columns: new[] { "AccountingYearId", "Num" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LigneInventaire_InventaireId",
                table: "LigneInventaire",
                column: "InventaireId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneInventaire_ProduitRefe",
                table: "LigneInventaire",
                column: "ProduitRefe");

            migrationBuilder.CreateIndex(
                name: "IX_LigneInventaire_RefProduit",
                table: "LigneInventaire",
                column: "RefProduit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LigneInventaire");

            migrationBuilder.DropTable(
                name: "Inventaire");
        }
    }
}
