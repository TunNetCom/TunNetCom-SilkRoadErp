using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRetourFournisseurReceptionFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_reception",
                table: "LigneRetourMarchandiseFournisseur",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "qte_recue",
                table: "LigneRetourMarchandiseFournisseur",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "utilisateur_reception",
                table: "LigneRetourMarchandiseFournisseur",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReceptionRetourFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetourMarchandiseFournisseurId = table.Column<int>(type: "int", nullable: false),
                    date_reception = table.Column<DateTime>(type: "datetime", nullable: false),
                    utilisateur = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.ReceptionRetourFournisseur", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ReceptionRetourFournisseur_dbo.RetourMarchandiseFournisseur_RetourMarchandiseFournisseurId",
                        column: x => x.RetourMarchandiseFournisseurId,
                        principalTable: "RetourMarchandiseFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionRetourFournisseur_RetourMarchandiseFournisseurId",
                table: "ReceptionRetourFournisseur",
                column: "RetourMarchandiseFournisseurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceptionRetourFournisseur");

            migrationBuilder.DropColumn(
                name: "date_reception",
                table: "LigneRetourMarchandiseFournisseur");

            migrationBuilder.DropColumn(
                name: "qte_recue",
                table: "LigneRetourMarchandiseFournisseur");

            migrationBuilder.DropColumn(
                name: "utilisateur_reception",
                table: "LigneRetourMarchandiseFournisseur");
        }
    }
}
