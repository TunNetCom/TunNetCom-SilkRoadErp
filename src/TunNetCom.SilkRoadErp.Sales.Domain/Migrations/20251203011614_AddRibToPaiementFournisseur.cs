using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRibToPaiementFournisseur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RibCle",
                table: "PaiementFournisseur",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RibCodeAgence",
                table: "PaiementFournisseur",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RibCodeEtab",
                table: "PaiementFournisseur",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RibNumeroCompte",
                table: "PaiementFournisseur",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RibCle",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "RibCodeAgence",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "RibCodeEtab",
                table: "PaiementFournisseur");

            migrationBuilder.DropColumn(
                name: "RibNumeroCompte",
                table: "PaiementFournisseur");
        }
    }
}
