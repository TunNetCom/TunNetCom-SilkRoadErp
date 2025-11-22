using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Créer la table AccountingYear d'abord
            migrationBuilder.CreateTable(
                name: "AccountingYear",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingYear", x => x.Id);
                });

            // 2. Créer l'index unique sur Year
            migrationBuilder.CreateIndex(
                name: "IX_AccountingYear_Year",
                table: "AccountingYear",
                column: "Year",
                unique: true);

            // 3. Ajouter les colonnes AccountingYearId (nullable temporairement)
            migrationBuilder.AddColumn<int>(
                name: "AccountingYearId",
                table: "FactureFournisseur",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountingYearId",
                table: "Facture",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountingYearId",
                table: "BonDeReception",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountingYearId",
                table: "BonDeLivraison",
                type: "int",
                nullable: true);

            // 4. Créer un exercice comptable par défaut (année actuelle)
            migrationBuilder.Sql(@"
                INSERT INTO AccountingYear (Year, IsActive)
                VALUES (YEAR(GETDATE()), 1);
            ");

            // 5. Remplir les AccountingYearId existants avec l'exercice par défaut
            migrationBuilder.Sql(@"
                UPDATE Facture
                SET AccountingYearId = (SELECT TOP 1 Id FROM AccountingYear WHERE IsActive = 1)
                WHERE AccountingYearId IS NULL;

                UPDATE FactureFournisseur
                SET AccountingYearId = (SELECT TOP 1 Id FROM AccountingYear WHERE IsActive = 1)
                WHERE AccountingYearId IS NULL;

                UPDATE BonDeLivraison
                SET AccountingYearId = (SELECT TOP 1 Id FROM AccountingYear WHERE IsActive = 1)
                WHERE AccountingYearId IS NULL;

                UPDATE BonDeReception
                SET AccountingYearId = (SELECT TOP 1 Id FROM AccountingYear WHERE IsActive = 1)
                WHERE AccountingYearId IS NULL;
            ");

            // 6. Rendre les colonnes NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "AccountingYearId",
                table: "FactureFournisseur",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountingYearId",
                table: "Facture",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountingYearId",
                table: "BonDeReception",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountingYearId",
                table: "BonDeLivraison",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 7. Créer les index pour les Foreign Keys
            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseur_AccountingYearId",
                table: "FactureFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_AccountingYearId",
                table: "Facture",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_AccountingYearId",
                table: "BonDeReception",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_AccountingYearId",
                table: "BonDeLivraison",
                column: "AccountingYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.BonDeLivraison_dbo.AccountingYear_AccountingYearId",
                table: "BonDeLivraison",
                column: "AccountingYearId",
                principalTable: "AccountingYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.BonDeReception_dbo.AccountingYear_AccountingYearId",
                table: "BonDeReception",
                column: "AccountingYearId",
                principalTable: "AccountingYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Facture_dbo.AccountingYear_AccountingYearId",
                table: "Facture",
                column: "AccountingYearId",
                principalTable: "AccountingYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.FactureFournisseur_dbo.AccountingYear_AccountingYearId",
                table: "FactureFournisseur",
                column: "AccountingYearId",
                principalTable: "AccountingYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.BonDeLivraison_dbo.AccountingYear_AccountingYearId",
                table: "BonDeLivraison");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.BonDeReception_dbo.AccountingYear_AccountingYearId",
                table: "BonDeReception");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Facture_dbo.AccountingYear_AccountingYearId",
                table: "Facture");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.FactureFournisseur_dbo.AccountingYear_AccountingYearId",
                table: "FactureFournisseur");

            migrationBuilder.DropTable(
                name: "AccountingYear");

            migrationBuilder.DropIndex(
                name: "IX_FactureFournisseur_AccountingYearId",
                table: "FactureFournisseur");

            migrationBuilder.DropIndex(
                name: "IX_Facture_AccountingYearId",
                table: "Facture");

            migrationBuilder.DropIndex(
                name: "IX_BonDeReception_AccountingYearId",
                table: "BonDeReception");

            migrationBuilder.DropIndex(
                name: "IX_BonDeLivraison_AccountingYearId",
                table: "BonDeLivraison");

            migrationBuilder.DropColumn(
                name: "AccountingYearId",
                table: "FactureFournisseur");

            migrationBuilder.DropColumn(
                name: "AccountingYearId",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "AccountingYearId",
                table: "BonDeReception");

            migrationBuilder.DropColumn(
                name: "AccountingYearId",
                table: "BonDeLivraison");
        }
    }
}
