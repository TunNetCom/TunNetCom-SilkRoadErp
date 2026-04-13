using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRibAndBankInfoInSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "banqueEntreprise",
                table: "Systeme",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ribCle",
                table: "Systeme",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ribCodeAgence",
                table: "Systeme",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ribCodeEtab",
                table: "Systeme",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ribNumeroCompte",
                table: "Systeme",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "banqueEntreprise",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "ribCle",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "ribCodeAgence",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "ribCodeEtab",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "ribNumeroCompte",
                table: "Systeme");
        }
    }
}
