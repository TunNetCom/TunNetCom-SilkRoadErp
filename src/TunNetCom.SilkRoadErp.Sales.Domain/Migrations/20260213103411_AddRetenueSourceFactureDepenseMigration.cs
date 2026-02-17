using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRetenueSourceFactureDepenseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RetenueSourceFactureDepense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactureDepenseId = table.Column<int>(type: "int", nullable: false),
                    NumTej = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MontantAvantRetenu = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TauxRetenu = table.Column<double>(type: "float", nullable: false),
                    MontantApresRetenu = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PdfStoragePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetenueSourceFactureDepense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetenueSourceFactureDepense_AccountingYear",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetenueSourceFactureDepense_FactureDepense",
                        column: x => x.FactureDepenseId,
                        principalTable: "FactureDepense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetenueSourceFactureDepense_AccountingYearId",
                table: "RetenueSourceFactureDepense",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_RetenueSourceFactureDepense_FactureDepenseId",
                table: "RetenueSourceFactureDepense",
                column: "FactureDepenseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetenueSourceFactureDepense");
        }
    }
}
