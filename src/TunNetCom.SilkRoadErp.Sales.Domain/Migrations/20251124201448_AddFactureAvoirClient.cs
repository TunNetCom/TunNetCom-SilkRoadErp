using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFactureAvoirClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Num_FactureAvoirClient",
                table: "Avoirs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FactureAvoirClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    Num_FactureAvoirClientSurPage = table.Column<int>(type: "int", nullable: false),
                    id_client = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Num_Facture = table.Column<int>(type: "int", nullable: true),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.FactureAvoirClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.FactureAvoirClient_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.FactureAvoirClient_dbo.Client_id_client",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.FactureAvoirClient_dbo.Facture_Num_Facture",
                        column: x => x.Num_Facture,
                        principalTable: "Facture",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_Num_FactureAvoirClient",
                table: "Avoirs",
                column: "Num_FactureAvoirClient");

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirClient_AccountingYearId",
                table: "FactureAvoirClient",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirClient_id_client",
                table: "FactureAvoirClient",
                column: "id_client");

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirClient_Num",
                table: "FactureAvoirClient",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirClient_Num_Facture",
                table: "FactureAvoirClient",
                column: "Num_Facture");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Avoirs_dbo.FactureAvoirClient_Num_FactureAvoirClient",
                table: "Avoirs",
                column: "Num_FactureAvoirClient",
                principalTable: "FactureAvoirClient",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Avoirs_dbo.FactureAvoirClient_Num_FactureAvoirClient",
                table: "Avoirs");

            migrationBuilder.DropTable(
                name: "FactureAvoirClient");

            migrationBuilder.DropIndex(
                name: "IX_Avoirs_Num_FactureAvoirClient",
                table: "Avoirs");

            migrationBuilder.DropColumn(
                name: "Num_FactureAvoirClient",
                table: "Avoirs");
        }
    }
}
