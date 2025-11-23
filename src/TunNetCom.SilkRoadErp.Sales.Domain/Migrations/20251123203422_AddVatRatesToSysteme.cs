using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddVatRatesToSysteme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VatRate0",
                table: "Systeme",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate7",
                table: "Systeme",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 7m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate13",
                table: "Systeme",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 13m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate19",
                table: "Systeme",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 19m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatRate0",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "VatRate13",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "VatRate19",
                table: "Systeme");

            migrationBuilder.DropColumn(
                name: "VatRate7",
                table: "Systeme");
        }
    }
}
