using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilleProduitAndSousFamilleProduit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SousFamilleProduitId",
                table: "Produit",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FamilleProduit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.FamilleProduit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SousFamilleProduit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FamilleProduitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.SousFamilleProduit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.SousFamilleProduit_dbo.FamilleProduit_FamilleProduitId",
                        column: x => x.FamilleProduitId,
                        principalTable: "FamilleProduit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produit_SousFamilleProduitId",
                table: "Produit",
                column: "SousFamilleProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_SousFamilleProduit_FamilleProduitId",
                table: "SousFamilleProduit",
                column: "FamilleProduitId");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Produit_dbo.SousFamilleProduit_SousFamilleProduitId",
                table: "Produit",
                column: "SousFamilleProduitId",
                principalTable: "SousFamilleProduit",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Produit_dbo.SousFamilleProduit_SousFamilleProduitId",
                table: "Produit");

            migrationBuilder.DropTable(
                name: "SousFamilleProduit");

            migrationBuilder.DropTable(
                name: "FamilleProduit");

            migrationBuilder.DropIndex(
                name: "IX_Produit_SousFamilleProduitId",
                table: "Produit");

            migrationBuilder.DropColumn(
                name: "SousFamilleProduitId",
                table: "Produit");
        }
    }
}
