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
            // Create junction tables
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
                        name: "FK_PaiementClientFacture_PaiementClient",
                        column: x => x.PaiementClientId,
                        principalTable: "PaiementClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaiementClientFacture_Facture",
                        column: x => x.FactureId,
                        principalTable: "Facture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_PaiementClientBonDeLivraison_PaiementClient",
                        column: x => x.PaiementClientId,
                        principalTable: "PaiementClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaiementClientBonDeLivraison_BonDeLivraison",
                        column: x => x.BonDeLivraisonId,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_PaiementFournisseurFactureFournisseur_PaiementFournisseur",
                        column: x => x.PaiementFournisseurId,
                        principalTable: "PaiementFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaiementFournisseurFactureFournisseur_FactureFournisseur",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_PaiementFournisseurBonDeReception_PaiementFournisseur",
                        column: x => x.PaiementFournisseurId,
                        principalTable: "PaiementFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaiementFournisseurBonDeReception_BonDeReception",
                        column: x => x.BonDeReceptionId,
                        principalTable: "BonDeReception",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Migrate existing data
            migrationBuilder.Sql(@"
                -- Migrate PaiementClient FactureId to junction table
                INSERT INTO PaiementClientFacture (PaiementClientId, FactureId)
                SELECT Id, FactureId
                FROM PaiementClient
                WHERE FactureId IS NOT NULL;

                -- Migrate PaiementClient BonDeLivraisonId to junction table
                INSERT INTO PaiementClientBonDeLivraison (PaiementClientId, BonDeLivraisonId)
                SELECT Id, BonDeLivraisonId
                FROM PaiementClient
                WHERE BonDeLivraisonId IS NOT NULL;

                -- Migrate PaiementFournisseur FactureFournisseurId to junction table
                INSERT INTO PaiementFournisseurFactureFournisseur (PaiementFournisseurId, FactureFournisseurId)
                SELECT Id, FactureFournisseurId
                FROM PaiementFournisseur
                WHERE FactureFournisseurId IS NOT NULL;

                -- Migrate PaiementFournisseur BonDeReceptionId to junction table
                INSERT INTO PaiementFournisseurBonDeReception (PaiementFournisseurId, BonDeReceptionId)
                SELECT Id, BonDeReceptionId
                FROM PaiementFournisseur
                WHERE BonDeReceptionId IS NOT NULL;
            ");

            // Drop old foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur");

            migrationBuilder.DropForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur");

            // Drop old check constraints
            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaiementClient_Document",
                table: "PaiementClient");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaiementFournisseur_Document",
                table: "PaiementFournisseur");

            // Drop old columns
            migrationBuilder.DropColumn(
                name: "FactureId",
                table: "PaiementClient");

            migrationBuilder.DropColumn(
                name: "BonDeLivraisonId",
                table: "PaiementClient");

            migrationBuilder.DropColumn(
                name: "FactureFournisseurId",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "BonDeReceptionId",
                table: "PaiementFournisseur");

            // Create indexes for better performance
            migrationBuilder.CreateIndex(
                name: "IX_PaiementClientFacture_FactureId",
                table: "PaiementClientFacture",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClientBonDeLivraison_BonDeLivraisonId",
                table: "PaiementClientBonDeLivraison",
                column: "BonDeLivraisonId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseurFactureFournisseur_FactureFournisseurId",
                table: "PaiementFournisseurFactureFournisseur",
                column: "FactureFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseurBonDeReception_BonDeReceptionId",
                table: "PaiementFournisseurBonDeReception",
                column: "BonDeReceptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate old columns
            migrationBuilder.AddColumn<int>(
                name: "FactureId",
                table: "PaiementClient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BonDeLivraisonId",
                table: "PaiementClient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FactureFournisseurId",
                table: "PaiementFournisseur",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BonDeReceptionId",
                table: "PaiementFournisseur",
                type: "int",
                nullable: true);

            // Migrate data back (only first item from each junction table)
            migrationBuilder.Sql(@"
                -- Migrate back from junction tables (taking first item only)
                UPDATE PaiementClient
                SET FactureId = (
                    SELECT TOP 1 FactureId
                    FROM PaiementClientFacture
                    WHERE PaiementClientFacture.PaiementClientId = PaiementClient.Id
                )
                WHERE EXISTS (SELECT 1 FROM PaiementClientFacture WHERE PaiementClientFacture.PaiementClientId = PaiementClient.Id);

                UPDATE PaiementClient
                SET BonDeLivraisonId = (
                    SELECT TOP 1 BonDeLivraisonId
                    FROM PaiementClientBonDeLivraison
                    WHERE PaiementClientBonDeLivraison.PaiementClientId = PaiementClient.Id
                )
                WHERE EXISTS (SELECT 1 FROM PaiementClientBonDeLivraison WHERE PaiementClientBonDeLivraison.PaiementClientId = PaiementClient.Id);

                UPDATE PaiementFournisseur
                SET FactureFournisseurId = (
                    SELECT TOP 1 FactureFournisseurId
                    FROM PaiementFournisseurFactureFournisseur
                    WHERE PaiementFournisseurFactureFournisseur.PaiementFournisseurId = PaiementFournisseur.Id
                )
                WHERE EXISTS (SELECT 1 FROM PaiementFournisseurFactureFournisseur WHERE PaiementFournisseurFactureFournisseur.PaiementFournisseurId = PaiementFournisseur.Id);

                UPDATE PaiementFournisseur
                SET BonDeReceptionId = (
                    SELECT TOP 1 BonDeReceptionId
                    FROM PaiementFournisseurBonDeReception
                    WHERE PaiementFournisseurBonDeReception.PaiementFournisseurId = PaiementFournisseur.Id
                )
                WHERE EXISTS (SELECT 1 FROM PaiementFournisseurBonDeReception WHERE PaiementFournisseurBonDeReception.PaiementFournisseurId = PaiementFournisseur.Id);
            ");

            // Recreate old foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_Facture",
                table: "PaiementClient",
                column: "FactureId",
                principalTable: "Facture",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementClient_BonDeLivraison",
                table: "PaiementClient",
                column: "BonDeLivraisonId",
                principalTable: "BonDeLivraison",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_FactureFournisseur",
                table: "PaiementFournisseur",
                column: "FactureFournisseurId",
                principalTable: "FactureFournisseur",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaiementFournisseur_BonDeReception",
                table: "PaiementFournisseur",
                column: "BonDeReceptionId",
                principalTable: "BonDeReception",
                principalColumn: "Id");

            // Recreate old check constraints
            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaiementClient_Document",
                table: "PaiementClient",
                sql: "(FactureId IS NULL AND BonDeLivraisonId IS NULL) OR " +
                     "(FactureId IS NOT NULL AND BonDeLivraisonId IS NULL) OR " +
                     "(FactureId IS NULL AND BonDeLivraisonId IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaiementFournisseur_Document",
                table: "PaiementFournisseur",
                sql: "(FactureFournisseurId IS NULL AND BonDeReceptionId IS NULL) OR " +
                     "(FactureFournisseurId IS NOT NULL AND BonDeReceptionId IS NULL) OR " +
                     "(FactureFournisseurId IS NULL AND BonDeReceptionId IS NOT NULL)");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_PaiementFournisseurBonDeReception_BonDeReceptionId",
                table: "PaiementFournisseurBonDeReception");

            migrationBuilder.DropIndex(
                name: "IX_PaiementFournisseurFactureFournisseur_FactureFournisseurId",
                table: "PaiementFournisseurFactureFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_PaiementClientBonDeLivraison_BonDeLivraisonId",
                table: "PaiementClientBonDeLivraison");

            migrationBuilder.DropIndex(
                name: "IX_PaiementClientFacture_FactureId",
                table: "PaiementClientFacture");

            // Drop junction tables
            migrationBuilder.DropTable(
                name: "PaiementFournisseurBonDeReception");

            migrationBuilder.DropTable(
                name: "PaiementFournisseurFactureFournisseur");

            migrationBuilder.DropTable(
                name: "PaiementClientBonDeLivraison");

            migrationBuilder.DropTable(
                name: "PaiementClientFacture");
        }
    }
}

