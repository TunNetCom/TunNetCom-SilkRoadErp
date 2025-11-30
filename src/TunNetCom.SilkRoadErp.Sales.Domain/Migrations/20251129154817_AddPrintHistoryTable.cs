using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPrintHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    PrintMode = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    PrinterName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Copies = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDuplicata = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.PrintHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrintHistory_DocumentId",
                table: "PrintHistory",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintHistory_DocumentType",
                table: "PrintHistory",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_PrintHistory_DocumentType_DocumentId",
                table: "PrintHistory",
                columns: new[] { "DocumentType", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_PrintHistory_PrintedAt",
                table: "PrintHistory",
                column: "PrintedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PrintHistory_UserId",
                table: "PrintHistory",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintHistory");
        }
    }
}