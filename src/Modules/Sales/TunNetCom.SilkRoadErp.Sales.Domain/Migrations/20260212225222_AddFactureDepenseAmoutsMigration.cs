using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFactureDepenseAmoutsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseHT0",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseHT13",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseHT19",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseHT7",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontantTVA0",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontantTVA13",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontantTVA19",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontantTVA7",
                table: "FactureDepense",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseHT0",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "BaseHT13",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "BaseHT19",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "BaseHT7",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "MontantTVA0",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "MontantTVA13",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "MontantTVA19",
                table: "FactureDepense");

            migrationBuilder.DropColumn(
                name: "MontantTVA7",
                table: "FactureDepense");
        }
    }
}
