using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class EditPaiementFournisseurMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mois",
                table: "PaiementFournisseur",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaiementFournisseur_Mois",
                table: "PaiementFournisseur",
                sql: "Mois IS NULL OR (Mois >= 1 AND Mois <= 12)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaiementFournisseur_Mois",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "Mois",
                table: "PaiementFournisseur");
        }
    }
}
