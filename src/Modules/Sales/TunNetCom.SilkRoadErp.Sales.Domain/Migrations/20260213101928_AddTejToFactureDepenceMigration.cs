using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTejToFactureDepenceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExonereRetenueSource",
                table: "TiersDepenseFonctionnement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TejCertificatFactureDepense",
                columns: table => new
                {
                    FactureDepenseId = table.Column<int>(type: "int", nullable: false),
                    RefCertif = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TejCertificatFactureDepense", x => x.FactureDepenseId);
                    table.ForeignKey(
                        name: "FK_TejCertificatFactureDepense_FactureDepense_FactureDepenseId",
                        column: x => x.FactureDepenseId,
                        principalTable: "FactureDepense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TejCertificatFactureDepense");

            migrationBuilder.DropColumn(
                name: "ExonereRetenueSource",
                table: "TiersDepenseFonctionnement");
        }
    }
}
