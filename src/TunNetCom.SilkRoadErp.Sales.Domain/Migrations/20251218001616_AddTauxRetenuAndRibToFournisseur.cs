using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTauxRetenuAndRibToFournisseur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rib_cle",
                table: "Fournisseur",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rib_code_agence",
                table: "Fournisseur",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rib_code_etab",
                table: "Fournisseur",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rib_numero_compte",
                table: "Fournisseur",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "taux_retenu",
                table: "Fournisseur",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rib_cle",
                table: "Fournisseur");

            migrationBuilder.DropColumn(
                name: "rib_code_agence",
                table: "Fournisseur");

            migrationBuilder.DropColumn(
                name: "rib_code_etab",
                table: "Fournisseur");

            migrationBuilder.DropColumn(
                name: "rib_numero_compte",
                table: "Fournisseur");

            migrationBuilder.DropColumn(
                name: "taux_retenu",
                table: "Fournisseur");
        }
    }
}
