using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddInstallationTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstallationTechnicianId",
                table: "BonDeLivraison",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstallationTechnician",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallationTechnician", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_InstallationTechnicianId",
                table: "BonDeLivraison",
                column: "InstallationTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationTechnician_Nom",
                table: "InstallationTechnician",
                column: "Nom");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.BonDeLivraison_dbo.InstallationTechnician_InstallationTechnicianId",
                table: "BonDeLivraison",
                column: "InstallationTechnicianId",
                principalTable: "InstallationTechnician",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.BonDeLivraison_dbo.InstallationTechnician_InstallationTechnicianId",
                table: "BonDeLivraison");

            migrationBuilder.DropTable(
                name: "InstallationTechnician");

            migrationBuilder.DropIndex(
                name: "IX_BonDeLivraison_InstallationTechnicianId",
                table: "BonDeLivraison");

            migrationBuilder.DropColumn(
                name: "InstallationTechnicianId",
                table: "BonDeLivraison");
        }
    }
}
