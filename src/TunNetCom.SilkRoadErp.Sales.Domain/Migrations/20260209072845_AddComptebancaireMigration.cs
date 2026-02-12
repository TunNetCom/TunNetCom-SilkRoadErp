using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddComptebancaireMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompteBancaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BanqueId = table.Column<int>(type: "int", nullable: false),
                    CodeEtablissement = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodeAgence = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumeroCompte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CleRib = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompteBancaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompteBancaire_Banque_BanqueId",
                        column: x => x.BanqueId,
                        principalTable: "Banque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankTransactionImport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompteBancaireId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransactionImport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransactionImport_CompteBancaire_CompteBancaireId",
                        column: x => x.CompteBancaireId,
                        principalTable: "CompteBancaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankTransactionImportId = table.Column<int>(type: "int", nullable: false),
                    DateOperation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateValeur = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SageCompteGeneral = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SageLibelle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransaction_BankTransactionImport_BankTransactionImportId",
                        column: x => x.BankTransactionImportId,
                        principalTable: "BankTransactionImport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransaction_BankTransactionImportId",
                table: "BankTransaction",
                column: "BankTransactionImportId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactionImport_CompteBancaireId",
                table: "BankTransactionImport",
                column: "CompteBancaireId");

            migrationBuilder.CreateIndex(
                name: "IX_CompteBancaire_BanqueId",
                table: "CompteBancaire",
                column: "BanqueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankTransaction");

            migrationBuilder.DropTable(
                name: "BankTransactionImport");

            migrationBuilder.DropTable(
                name: "CompteBancaire");
        }
    }
}
