using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTejCertificatFactureMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TejCertificatFacture",
                columns: table => new
                {
                    FactureFournisseurId = table.Column<int>(type: "int", nullable: false),
                    RefCertif = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TejCertificatFacture", x => x.FactureFournisseurId);
                    table.ForeignKey(
                        name: "FK_TejCertificatFacture_FactureFournisseur_FactureFournisseurId",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TejCertificatSequence",
                columns: table => new
                {
                    Annee = table.Column<int>(type: "int", nullable: false),
                    Mois = table.Column<int>(type: "int", nullable: false),
                    DerniereSequence = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TejCertificatSequence", x => new { x.Annee, x.Mois });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TejCertificatFacture");

            migrationBuilder.DropTable(
                name: "TejCertificatSequence");
        }
    }
}
