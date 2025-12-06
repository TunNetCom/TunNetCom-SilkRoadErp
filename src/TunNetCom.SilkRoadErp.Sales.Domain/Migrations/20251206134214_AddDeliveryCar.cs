using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryCarId",
                table: "BonDeLivraison",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryCar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.DeliveryCar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_DeliveryCarId",
                table: "BonDeLivraison",
                column: "DeliveryCarId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryCar_Matricule",
                table: "DeliveryCar",
                column: "Matricule",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.BonDeLivraison_dbo.DeliveryCar_DeliveryCarId",
                table: "BonDeLivraison",
                column: "DeliveryCarId",
                principalTable: "DeliveryCar",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.BonDeLivraison_dbo.DeliveryCar_DeliveryCarId",
                table: "BonDeLivraison");

            migrationBuilder.DropTable(
                name: "DeliveryCar");

            migrationBuilder.DropIndex(
                name: "IX_BonDeLivraison_DeliveryCarId",
                table: "BonDeLivraison");

            migrationBuilder.DropColumn(
                name: "DeliveryCarId",
                table: "BonDeLivraison");
        }
    }
}
