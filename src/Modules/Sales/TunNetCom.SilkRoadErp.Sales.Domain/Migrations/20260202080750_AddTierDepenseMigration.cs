using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTierDepenseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiersDepenseFonctionnement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodeCat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EtbSec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiersDepenseFonctionnement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactureDepense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    IdTiersDepenseFonctionnement = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MontantTotal = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactureDepense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactureDepense_AccountingYear",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactureDepense_TiersDepenseFonctionnement",
                        column: x => x.IdTiersDepenseFonctionnement,
                        principalTable: "TiersDepenseFonctionnement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaiementTiersDepense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroTransactionBancaire = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TiersDepenseFonctionnementId = table.Column<int>(type: "int", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "datetime", nullable: false),
                    MethodePaiement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumeroChequeTraite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BanqueId = table.Column<int>(type: "int", nullable: true),
                    DateEcheance = table.Column<DateTime>(type: "datetime", nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RibCodeEtab = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RibCodeAgence = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RibNumeroCompte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RibCle = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Mois = table.Column<int>(type: "int", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementTiersDepense", x => x.Id);
                    table.CheckConstraint("CHK_PaiementTiersDepense_Mois", "Mois IS NULL OR (Mois >= 1 AND Mois <= 12)");
                    table.CheckConstraint("CHK_PaiementTiersDepense_Montant", "Montant > 0");
                    table.ForeignKey(
                        name: "FK_PaiementTiersDepense_AccountingYear",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementTiersDepense_Banque",
                        column: x => x.BanqueId,
                        principalTable: "Banque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaiementTiersDepense_TiersDepenseFonctionnement",
                        column: x => x.TiersDepenseFonctionnementId,
                        principalTable: "TiersDepenseFonctionnement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaiementTiersDepenseFactureDepense",
                columns: table => new
                {
                    PaiementTiersDepenseId = table.Column<int>(type: "int", nullable: false),
                    FactureDepenseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementTiersDepenseFactureDepense", x => new { x.PaiementTiersDepenseId, x.FactureDepenseId });
                    table.ForeignKey(
                        name: "FK_PaiementTiersDepenseFactureDepense_FactureDepense",
                        column: x => x.FactureDepenseId,
                        principalTable: "FactureDepense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementTiersDepenseFactureDepense_PaiementTiersDepense",
                        column: x => x.PaiementTiersDepenseId,
                        principalTable: "PaiementTiersDepense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FactureDepense_AccountingYearId",
                table: "FactureDepense",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureDepense_IdTiersDepenseFonctionnement",
                table: "FactureDepense",
                column: "IdTiersDepenseFonctionnement");

            migrationBuilder.CreateIndex(
                name: "IX_FactureDepense_Num_AccountingYearId",
                table: "FactureDepense",
                columns: new[] { "Num", "AccountingYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaiementTiersDepense_AccountingYearId",
                table: "PaiementTiersDepense",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementTiersDepense_BanqueId",
                table: "PaiementTiersDepense",
                column: "BanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementTiersDepense_DatePaiement",
                table: "PaiementTiersDepense",
                column: "DatePaiement");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementTiersDepense_TiersDepenseFonctionnementId",
                table: "PaiementTiersDepense",
                column: "TiersDepenseFonctionnementId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementTiersDepenseFactureDepense_FactureDepenseId",
                table: "PaiementTiersDepenseFactureDepense",
                column: "FactureDepenseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaiementTiersDepenseFactureDepense");

            migrationBuilder.DropTable(
                name: "FactureDepense");

            migrationBuilder.DropTable(
                name: "PaiementTiersDepense");

            migrationBuilder.DropTable(
                name: "TiersDepenseFonctionnement");
        }
    }
}
