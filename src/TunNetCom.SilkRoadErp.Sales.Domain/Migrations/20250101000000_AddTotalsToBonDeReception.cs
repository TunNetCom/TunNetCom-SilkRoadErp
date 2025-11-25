using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalsToBonDeReception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "tot_H_tva",
                table: "BonDeReception",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tot_tva",
                table: "BonDeReception",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "net_payer",
                table: "BonDeReception",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tot_H_tva",
                table: "BonDeReception");

            migrationBuilder.DropColumn(
                name: "tot_tva",
                table: "BonDeReception");

            migrationBuilder.DropColumn(
                name: "net_payer",
                table: "BonDeReception");
        }
    }
}

