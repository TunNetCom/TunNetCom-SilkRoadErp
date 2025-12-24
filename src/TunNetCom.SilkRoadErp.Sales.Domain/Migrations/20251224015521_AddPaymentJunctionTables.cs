using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentJunctionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create junction tables first (before dropping foreign keys)
            migrationBuilder.CreateTable(
                name: "PaiementClientFacture",
                columns: table => new
                {
                    PaiementClientId = table.Column<int>(type: "int", nullable: false),
                    FactureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementClientFacture", x => new { x.PaiementClientId, x.FactureId });
                    table.ForeignKey(
                        name: "FK_PaiementClientFacture_Facture",
                        column: x => x.FactureId,
                        principalTable: "Facture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementClientFacture_PaiementClient",
                        column: x => x.PaiementClientId,
                        principalTable: "PaiementClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaiementClientBonDeLivraison",
                columns: table => new
                {
                    PaiementClientId = table.Column<int>(type: "int", nullable: false),
                    BonDeLivraisonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementClientBonDeLivraison", x => new { x.PaiementClientId, x.BonDeLivraisonId });
                    table.ForeignKey(
                        name: "FK_PaiementClientBonDeLivraison_BonDeLivraison",
                        column: x => x.BonDeLivraisonId,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementClientBonDeLivraison_PaiementClient",
                        column: x => x.PaiementClientId,
                        principalTable: "PaiementClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaiementFournisseurFactureFournisseur",
                columns: table => new
                {
                    PaiementFournisseurId = table.Column<int>(type: "int", nullable: false),
                    FactureFournisseurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementFournisseurFactureFournisseur", x => new { x.PaiementFournisseurId, x.FactureFournisseurId });
                    table.ForeignKey(
                        name: "FK_PaiementFournisseurFactureFournisseur_FactureFournisseur",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementFournisseurFactureFournisseur_PaiementFournisseur",
                        column: x => x.PaiementFournisseurId,
                        principalTable: "PaiementFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaiementFournisseurBonDeReception",
                columns: table => new
                {
                    PaiementFournisseurId = table.Column<int>(type: "int", nullable: false),
                    BonDeReceptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementFournisseurBonDeReception", x => new { x.PaiementFournisseurId, x.BonDeReceptionId });
                    table.ForeignKey(
                        name: "FK_PaiementFournisseurBonDeReception_BonDeReception",
                        column: x => x.BonDeReceptionId,
                        principalTable: "BonDeReception",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementFournisseurBonDeReception_PaiementFournisseur",
                        column: x => x.PaiementFournisseurId,
                        principalTable: "PaiementFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate existing data from old columns to new junction tables
            migrationBuilder.Sql(
                @"INSERT INTO PaiementClientFacture (PaiementClientId, FactureId)
                  SELECT Id, FactureId FROM PaiementClient WHERE FactureId IS NOT NULL;");

            migrationBuilder.Sql(
                @"INSERT INTO PaiementClientBonDeLivraison (PaiementClientId, BonDeLivraisonId)
                  SELECT Id, BonDeLivraisonId FROM PaiementClient WHERE BonDeLivraisonId IS NOT NULL;");

            migrationBuilder.Sql(
                @"INSERT INTO PaiementFournisseurFactureFournisseur (PaiementFournisseurId, FactureFournisseurId)
                  SELECT Id, FactureFournisseurId FROM PaiementFournisseur WHERE FactureFournisseurId IS NOT NULL;");

            migrationBuilder.Sql(
                @"INSERT INTO PaiementFournisseurBonDeReception (PaiementFournisseurId, BonDeReceptionId)
                  SELECT Id, BonDeReceptionId FROM PaiementFournisseur WHERE BonDeReceptionId IS NOT NULL;");

            // Now drop foreign keys and constraints
            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_PaiementFournisseur_BonDeReceptionId",
                table: "PaiementFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_PaiementFournisseur_FactureFournisseurId",
                table: "PaiementFournisseur");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaiementFournisseur_Document",
                table: "PaiementFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_PaiementClient_BonDeLivraisonId",
                table: "PaiementClient");

            migrationBuilder.DropIndex(
                name: "IX_PaiementClient_FactureId",
                table: "PaiementClient");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaiementClient_Document",
                table: "PaiementClient");

            migrationBuilder.DropColumn(
                name: "BonDeReceptionId",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "FactureFournisseurId",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "BonDeLivraisonId",
                table: "PaiementClient");

            migrationBuilder.DropColumn(
                name: "FactureId",
                table: "PaiementClient");

            migrationBuilder.AddColumn<string>(
                name: "DocumentStoragePath",
                table: "PaiementFournisseur",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentStoragePath",
                table: "PaiementClient",
                type: "nvarchar(max)",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_PaiementClientBonDeLivraison_BonDeLivraisonId",
                table: "PaiementClientBonDeLivraison",
                column: "BonDeLivraisonId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClientFacture_FactureId",
                table: "PaiementClientFacture",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseurBonDeReception_BonDeReceptionId",
                table: "PaiementFournisseurBonDeReception",
                column: "BonDeReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseurFactureFournisseur_FactureFournisseurId",
                table: "PaiementFournisseurFactureFournisseur",
                column: "FactureFournisseurId");

            // Note: Exclusivity constraint is enforced at application level (in validators and handlers)
            // SQL Server check constraints cannot use subqueries, so we rely on application-level validation
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaiementClientBonDeLivraison");

            migrationBuilder.DropTable(
                name: "PaiementClientFacture");

            migrationBuilder.DropTable(
                name: "PaiementFournisseurBonDeReception");

            migrationBuilder.DropTable(
                name: "PaiementFournisseurFactureFournisseur");

            migrationBuilder.DropColumn(
                name: "DocumentStoragePath",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "DocumentStoragePath",
                table: "PaiementClient");

            migrationBuilder.AddColumn<int>(
                name: "BonDeReceptionId",
                table: "PaiementFournisseur",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FactureFournisseurId",
                table: "PaiementFournisseur",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BonDeLivraisonId",
                table: "PaiementClient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FactureId",
                table: "PaiementClient",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_BonDeReceptionId",
                table: "PaiementFournisseur",
                column: "BonDeReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_FactureFournisseurId",
                table: "PaiementFournisseur",
                column: "FactureFournisseurId");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaiementFournisseur_Document",
                table: "PaiementFournisseur",
                sql: "(FactureFournisseurId IS NULL AND BonDeReceptionId IS NULL) OR (FactureFournisseurId IS NOT NULL AND BonDeReceptionId IS NULL) OR (FactureFournisseurId IS NULL AND BonDeReceptionId IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_BonDeLivraisonId",
                table: "PaiementClient",
                column: "BonDeLivraisonId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_FactureId",
                table: "PaiementClient",
                column: "FactureId");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaiementClient_Document",
                table: "PaiementClient",
                sql: "(FactureId IS NULL AND BonDeLivraisonId IS NULL) OR (FactureId IS NOT NULL AND BonDeLivraisonId IS NULL) OR (FactureId IS NULL AND BonDeLivraisonId IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient",
                column: "BonDeLivraisonId",
                principalTable: "BonDeLivraison",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient",
                column: "FactureId",
                principalTable: "Facture",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur",
                column: "BonDeReceptionId",
                principalTable: "BonDeReception",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur",
                column: "FactureFournisseurId",
                principalTable: "FactureFournisseur",
                principalColumn: "Id");
        }
    }
}
