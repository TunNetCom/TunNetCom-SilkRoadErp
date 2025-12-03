using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountingYear",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingYear", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banque", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    adresse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    matricule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code_cat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    etb_sec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Client", x => x.Id);
                });

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
                name: "Fournisseur",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    code_cat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    etb_sec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mail_deux = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    constructeur = table.Column<bool>(type: "bit", nullable: false),
                    adresse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Fournisseur", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Systeme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NomSociete = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timbre = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    matriculeFiscale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    codeTVA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    codeCategorie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    etbSecondaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pourcentageFodec = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    adresseRetenu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pourcentageRetenu = table.Column<double>(type: "float", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate0 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate7 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate13 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate19 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BloquerVenteStockInsuffisant = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DecimalPlaces = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    SeuilRetenueSource = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1000m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Systeme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    DateInventaire = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Inventaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Inventaire_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devis",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_client = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    tot_H_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tot_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tot_ttc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Devis", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.Devis_dbo.Client_id_client",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    id_client = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Facture", x => x.Id);
                    table.UniqueConstraint("AK_Facture_Num", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.Facture_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Facture_dbo.Client_id_client",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "Id");
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

            migrationBuilder.CreateTable(
                name: "Commandes",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    fournisseurId = table.Column<int>(type: "int", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Commandes", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.Commandes_dbo.Fournisseur_fournisseurId",
                        column: x => x.fournisseurId,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "EcheanceDesFournisseurs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateEcheance = table.Column<DateTime>(type: "datetime", nullable: false),
                    numCheque = table.Column<long>(type: "bigint", nullable: false),
                    montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    fournisseur_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.EcheanceDesFournisseurs", x => x.id);
                    table.ForeignKey(
                        name: "FK_dbo.EcheanceDesFournisseurs_dbo.Fournisseur_fournisseur_id",
                        column: x => x.fournisseur_id,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "FactureFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    paye = table.Column<bool>(type: "bit", nullable: false),
                    NumFactureFournisseur = table.Column<long>(type: "bigint", nullable: false),
                    dateFacturationFournisseur = table.Column<DateTime>(type: "datetime", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.FactureFournisseur", x => x.Id);
                    table.UniqueConstraint("AK_FactureFournisseur_Num", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.FactureFournisseur_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.FactureFournisseur_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.DocumentTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DocumentTag_dbo.Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonDeLivraison",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    tot_H_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tot_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    net_payer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    temp_bl = table.Column<TimeOnly>(type: "time", nullable: false),
                    Num_Facture = table.Column<int>(type: "int", nullable: true),
                    clientId = table.Column<int>(type: "int", nullable: true),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    InstallationTechnicianId = table.Column<int>(type: "int", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.BonDeLivraison", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.BonDeLivraison_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.BonDeLivraison_dbo.Client_clientId",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.BonDeLivraison_dbo.Facture_Num_Facture",
                        column: x => x.Num_Facture,
                        principalTable: "Facture",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.BonDeLivraison_dbo.InstallationTechnician_InstallationTechnicianId",
                        column: x => x.InstallationTechnicianId,
                        principalTable: "InstallationTechnician",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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
                        principalColumn: "Num");
                });

            migrationBuilder.CreateTable(
                name: "RetenueSourceClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumFacture = table.Column<int>(type: "int", nullable: false),
                    NumTej = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MontantAvantRetenu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TauxRetenu = table.Column<double>(type: "float", nullable: false),
                    MontantApresRetenu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdfStoragePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.RetenueSourceClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.RetenueSourceClient_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.RetenueSourceClient_dbo.Facture_NumFacture",
                        column: x => x.NumFacture,
                        principalTable: "Facture",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Produit",
                columns: table => new
                {
                    refe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qteLimite = table.Column<int>(type: "int", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    remiseAchat = table.Column<double>(type: "float", nullable: false),
                    TVA = table.Column<double>(type: "float", nullable: false),
                    prix = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    prixAchat = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    visibilite = table.Column<bool>(type: "bit", nullable: false),
                    SousFamilleProduitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Produit", x => x.refe);
                    table.ForeignKey(
                        name: "FK_dbo.Produit_dbo.SousFamilleProduit_SousFamilleProduitId",
                        column: x => x.SousFamilleProduitId,
                        principalTable: "SousFamilleProduit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AvoirFinancierFournisseurs",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false),
                    NumSurPage = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tot_ttc = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.AvoirFinancierFournisseurs", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_Num",
                        column: x => x.Num,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonDeReception",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    Num_Bon_fournisseur = table.Column<long>(type: "bigint", nullable: false),
                    date_livraison = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Num_Facture_fournisseur = table.Column<int>(type: "int", nullable: true),
                    tot_H_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tot_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    net_payer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.BonDeReception", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.BonDeReception_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.BonDeReception_dbo.FactureFournisseur_Num_Facture_fournisseur",
                        column: x => x.Num_Facture_fournisseur,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.BonDeReception_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "FactureAvoirFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    Num_FactureAvoirFourSurPAge = table.Column<int>(type: "int", nullable: false),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Num_FactureFournisseur = table.Column<int>(type: "int", nullable: true),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.FactureAvoirFournisseur", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.FactureAvoirFournisseur_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_Num_FactureFournisseur",
                        column: x => x.Num_FactureFournisseur,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.FactureAvoirFournisseur_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "RetenueSourceFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumFactureFournisseur = table.Column<int>(type: "int", nullable: false),
                    NumTej = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MontantAvantRetenu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TauxRetenu = table.Column<double>(type: "float", nullable: false),
                    MontantApresRetenu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdfStoragePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.RetenueSourceFournisseur", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.RetenueSourceFournisseur_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.RetenueSourceFournisseur_dbo.FactureFournisseur_NumFactureFournisseur",
                        column: x => x.NumFactureFournisseur,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaiementClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "datetime", nullable: false),
                    MethodePaiement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FactureId = table.Column<int>(type: "int", nullable: true),
                    BonDeLivraisonId = table.Column<int>(type: "int", nullable: true),
                    NumeroChequeTraite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BanqueId = table.Column<int>(type: "int", nullable: true),
                    DateEcheance = table.Column<DateTime>(type: "datetime", nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementClient", x => x.Id);
                    table.CheckConstraint("CHK_PaiementClient_Document", "(FactureId IS NULL AND BonDeLivraisonId IS NULL) OR (FactureId IS NOT NULL AND BonDeLivraisonId IS NULL) OR (FactureId IS NULL AND BonDeLivraisonId IS NOT NULL)");
                    table.CheckConstraint("CHK_PaiementClient_Montant", "Montant > 0");
                    table.ForeignKey(
                        name: "FK_PaiementClient_AccountingYear",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementClient_Banque",
                        column: x => x.BanqueId,
                        principalTable: "Banque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaiementClient_BonDeLivraison",
                        column: x => x.BonDeLivraisonId,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaiementClient_Client",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementClient_Facture",
                        column: x => x.FactureId,
                        principalTable: "Facture",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    BonDeLivraisonId = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    date_tr = table.Column<DateTime>(type: "datetime", nullable: false),
                    montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Transaction", x => x.BonDeLivraisonId);
                    table.ForeignKey(
                        name: "FK_dbo.Transaction_dbo.BonDeLivraison_BonDeLivraisonId",
                        column: x => x.BonDeLivraisonId,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Avoirs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: true),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Num_FactureAvoirClient = table.Column<int>(type: "int", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Avoirs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Avoirs_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Avoirs_dbo.Client_clientId",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Avoirs_dbo.FactureAvoirClient_Num_FactureAvoirClient",
                        column: x => x.Num_FactureAvoirClient,
                        principalTable: "FactureAvoirClient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LigneBL",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonDeLivraisonId = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    qte_livree = table.Column<int>(type: "int", nullable: true),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneBL", x => x.Id_li);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBL_dbo.BonDeLivraison_BonDeLivraisonId",
                        column: x => x.BonDeLivraisonId,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBL_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "LigneCommandes",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_commande = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneCommandes", x => x.Id_li);
                    table.ForeignKey(
                        name: "FK_dbo.LigneCommandes_dbo.Commandes_Num_commande",
                        column: x => x.Num_commande,
                        principalTable: "Commandes",
                        principalColumn: "Num");
                    table.ForeignKey(
                        name: "FK_dbo.LigneCommandes_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "LigneDevis",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DevisId = table.Column<int>(type: "int", nullable: false),
                    Ref_produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneDevis", x => x.Id_li);
                    table.ForeignKey(
                        name: "FK_dbo.LigneDevis_dbo.Devis_DevisId",
                        column: x => x.DevisId,
                        principalTable: "Devis",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneDevis_dbo.Produit_Ref_produit",
                        column: x => x.Ref_produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "LigneInventaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventaireId = table.Column<int>(type: "int", nullable: false),
                    RefProduit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantiteTheorique = table.Column<int>(type: "int", nullable: false),
                    QuantiteReelle = table.Column<int>(type: "int", nullable: false),
                    PrixHt = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DernierPrixAchat = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ProduitRefe = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneInventaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LigneInventaire_Produit_ProduitRefe",
                        column: x => x.ProduitRefe,
                        principalTable: "Produit",
                        principalColumn: "refe");
                    table.ForeignKey(
                        name: "FK_dbo.LigneInventaire_dbo.Inventaire_InventaireId",
                        column: x => x.InventaireId,
                        principalTable: "Inventaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneInventaire_dbo.Produit_RefProduit",
                        column: x => x.RefProduit,
                        principalTable: "Produit",
                        principalColumn: "refe",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LigneBonReception",
                columns: table => new
                {
                    Id_ligne = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonDeReceptionId = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneBonReception", x => x.Id_ligne);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBonReception_dbo.BonDeReception_BonDeReceptionId",
                        column: x => x.BonDeReceptionId,
                        principalTable: "BonDeReception",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBonReception_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "PaiementFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FournisseurId = table.Column<int>(type: "int", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "datetime", nullable: false),
                    MethodePaiement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FactureFournisseurId = table.Column<int>(type: "int", nullable: true),
                    BonDeReceptionId = table.Column<int>(type: "int", nullable: true),
                    NumeroChequeTraite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BanqueId = table.Column<int>(type: "int", nullable: true),
                    DateEcheance = table.Column<DateTime>(type: "datetime", nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RibCodeEtab = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RibCodeAgence = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RibNumeroCompte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RibCle = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementFournisseur", x => x.Id);
                    table.CheckConstraint("CHK_PaiementFournisseur_Document", "(FactureFournisseurId IS NULL AND BonDeReceptionId IS NULL) OR (FactureFournisseurId IS NOT NULL AND BonDeReceptionId IS NULL) OR (FactureFournisseurId IS NULL AND BonDeReceptionId IS NOT NULL)");
                    table.CheckConstraint("CHK_PaiementFournisseur_Montant", "Montant > 0");
                    table.ForeignKey(
                        name: "FK_PaiementFournisseur_AccountingYear",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaiementFournisseur_Banque",
                        column: x => x.BanqueId,
                        principalTable: "Banque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaiementFournisseur_BonDeReception",
                        column: x => x.BonDeReceptionId,
                        principalTable: "BonDeReception",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaiementFournisseur_FactureFournisseur",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaiementFournisseur_Fournisseur",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvoirFournisseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    fournisseurId = table.Column<int>(type: "int", nullable: true),
                    Num_FactureAvoirFournisseur = table.Column<int>(type: "int", nullable: true),
                    Num_AvoirFournisseur = table.Column<int>(type: "int", nullable: false),
                    AccountingYearId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.AvoirFournisseur", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.AccountingYear_AccountingYearId",
                        column: x => x.AccountingYearId,
                        principalTable: "AccountingYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_Num_FactureAvoirFournisseur",
                        column: x => x.Num_FactureAvoirFournisseur,
                        principalTable: "FactureAvoirFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.Fournisseur_fournisseurId",
                        column: x => x.fournisseurId,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "LigneAvoirs",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvoirsId = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneAvoirs", x => x.Id_li);
                    table.ForeignKey(
                        name: "FK_dbo.LigneAvoirs_dbo.Avoirs_AvoirsId",
                        column: x => x.AvoirsId,
                        principalTable: "Avoirs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.LigneAvoirs_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "LigneAvoirFournisseur",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvoirFournisseurId = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.LigneAvoirFournisseur", x => x.Id_li);
                    table.ForeignKey(
                        name: "FK_dbo.LigneAvoirFournisseur_dbo.AvoirFournisseur_AvoirFournisseurId",
                        column: x => x.AvoirFournisseurId,
                        principalTable: "AvoirFournisseur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneAvoirFournisseur_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingYear_Year",
                table: "AccountingYear",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityId",
                table: "AuditLog",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityName",
                table: "AuditLog",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityName_EntityId",
                table: "AuditLog",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityName_EntityId_Timestamp",
                table: "AuditLog",
                columns: new[] { "EntityName", "EntityId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_Timestamp",
                table: "AuditLog",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_UserId",
                table: "AuditLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_AccountingYearId",
                table: "AvoirFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_fournisseurId",
                table: "AvoirFournisseur",
                column: "fournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_Num",
                table: "AvoirFournisseur",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur",
                column: "Num_FactureAvoirFournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_AccountingYearId",
                table: "Avoirs",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_clientId",
                table: "Avoirs",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_Num",
                table: "Avoirs",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_Num_FactureAvoirClient",
                table: "Avoirs",
                column: "Num_FactureAvoirClient");

            migrationBuilder.CreateIndex(
                name: "IX_Banque_Nom",
                table: "Banque",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_AccountingYearId",
                table: "BonDeLivraison",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_clientId",
                table: "BonDeLivraison",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_InstallationTechnicianId",
                table: "BonDeLivraison",
                column: "InstallationTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_Num",
                table: "BonDeLivraison",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_Num_Facture",
                table: "BonDeLivraison",
                column: "Num_Facture");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_AccountingYearId",
                table: "BonDeReception",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_id_fournisseur",
                table: "BonDeReception",
                column: "id_fournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_Num",
                table: "BonDeReception",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_Num_Facture_fournisseur",
                table: "BonDeReception",
                column: "Num_Facture_fournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_fournisseurId",
                table: "Commandes",
                column: "fournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Devis_id_client",
                table: "Devis",
                column: "id_client");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTag_DocumentType_DocumentId",
                table: "DocumentTag",
                columns: new[] { "DocumentType", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTag_DocumentType_DocumentId_TagId",
                table: "DocumentTag",
                columns: new[] { "DocumentType", "DocumentId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTag_TagId",
                table: "DocumentTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_EcheanceDesFournisseurs_fournisseur_id",
                table: "EcheanceDesFournisseurs",
                column: "fournisseur_id");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_AccountingYearId",
                table: "Facture",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_id_client",
                table: "Facture",
                column: "id_client");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_Num",
                table: "Facture",
                column: "Num",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_AccountingYearId",
                table: "FactureAvoirFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_id_fournisseur",
                table: "FactureAvoirFournisseur",
                column: "id_fournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_Num",
                table: "FactureAvoirFournisseur",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_Num_FactureFournisseur",
                table: "FactureAvoirFournisseur",
                column: "Num_FactureFournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseur_AccountingYearId",
                table: "FactureFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseur_id_fournisseur",
                table: "FactureFournisseur",
                column: "id_fournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseur_Num",
                table: "FactureFournisseur",
                column: "Num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallationTechnician_Nom",
                table: "InstallationTechnician",
                column: "Nom");

            migrationBuilder.CreateIndex(
                name: "IX_Inventaire_AccountingYearId",
                table: "Inventaire",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventaire_AccountingYearId_Num",
                table: "Inventaire",
                columns: new[] { "AccountingYearId", "Num" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirFournisseur_AvoirFournisseurId",
                table: "LigneAvoirFournisseur",
                column: "AvoirFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirFournisseur_Ref_Produit",
                table: "LigneAvoirFournisseur",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirs_AvoirsId",
                table: "LigneAvoirs",
                column: "AvoirsId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirs_Ref_Produit",
                table: "LigneAvoirs",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBL_BonDeLivraisonId",
                table: "LigneBL",
                column: "BonDeLivraisonId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBL_Ref_Produit",
                table: "LigneBL",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBonReception_BonDeReceptionId",
                table: "LigneBonReception",
                column: "BonDeReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBonReception_Ref_Produit",
                table: "LigneBonReception",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneCommandes_Num_commande",
                table: "LigneCommandes",
                column: "Num_commande");

            migrationBuilder.CreateIndex(
                name: "IX_LigneCommandes_Ref_Produit",
                table: "LigneCommandes",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneDevis_DevisId",
                table: "LigneDevis",
                column: "DevisId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneDevis_Ref_produit",
                table: "LigneDevis",
                column: "Ref_produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneInventaire_InventaireId",
                table: "LigneInventaire",
                column: "InventaireId");

            migrationBuilder.CreateIndex(
                name: "IX_LigneInventaire_ProduitRefe",
                table: "LigneInventaire",
                column: "ProduitRefe");

            migrationBuilder.CreateIndex(
                name: "IX_LigneInventaire_RefProduit",
                table: "LigneInventaire",
                column: "RefProduit");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_AccountingYearId",
                table: "PaiementClient",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_BanqueId",
                table: "PaiementClient",
                column: "BanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_BonDeLivraisonId",
                table: "PaiementClient",
                column: "BonDeLivraisonId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_ClientId",
                table: "PaiementClient",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_DatePaiement",
                table: "PaiementClient",
                column: "DatePaiement");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_FactureId",
                table: "PaiementClient",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementClient_Numero",
                table: "PaiementClient",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_AccountingYearId",
                table: "PaiementFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_BanqueId",
                table: "PaiementFournisseur",
                column: "BanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_BonDeReceptionId",
                table: "PaiementFournisseur",
                column: "BonDeReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_DatePaiement",
                table: "PaiementFournisseur",
                column: "DatePaiement");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_FactureFournisseurId",
                table: "PaiementFournisseur",
                column: "FactureFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_FournisseurId",
                table: "PaiementFournisseur",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementFournisseur_Numero",
                table: "PaiementFournisseur",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Produit_SousFamilleProduitId",
                table: "Produit",
                column: "SousFamilleProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RetenueSourceClient_AccountingYearId",
                table: "RetenueSourceClient",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_RetenueSourceClient_NumFacture",
                table: "RetenueSourceClient",
                column: "NumFacture",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RetenueSourceFournisseur_AccountingYearId",
                table: "RetenueSourceFournisseur",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_RetenueSourceFournisseur_NumFactureFournisseur",
                table: "RetenueSourceFournisseur",
                column: "NumFactureFournisseur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SousFamilleProduit_FamilleProduitId",
                table: "SousFamilleProduit",
                column: "FamilleProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "AvoirFinancierFournisseurs");

            migrationBuilder.DropTable(
                name: "DocumentTag");

            migrationBuilder.DropTable(
                name: "EcheanceDesFournisseurs");

            migrationBuilder.DropTable(
                name: "LigneAvoirFournisseur");

            migrationBuilder.DropTable(
                name: "LigneAvoirs");

            migrationBuilder.DropTable(
                name: "LigneBL");

            migrationBuilder.DropTable(
                name: "LigneBonReception");

            migrationBuilder.DropTable(
                name: "LigneCommandes");

            migrationBuilder.DropTable(
                name: "LigneDevis");

            migrationBuilder.DropTable(
                name: "LigneInventaire");

            migrationBuilder.DropTable(
                name: "PaiementClient");

            migrationBuilder.DropTable(
                name: "PaiementFournisseur");

            migrationBuilder.DropTable(
                name: "PrintHistory");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RetenueSourceClient");

            migrationBuilder.DropTable(
                name: "RetenueSourceFournisseur");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Systeme");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "AvoirFournisseur");

            migrationBuilder.DropTable(
                name: "Avoirs");

            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropTable(
                name: "Devis");

            migrationBuilder.DropTable(
                name: "Produit");

            migrationBuilder.DropTable(
                name: "Inventaire");

            migrationBuilder.DropTable(
                name: "Banque");

            migrationBuilder.DropTable(
                name: "BonDeReception");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "BonDeLivraison");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "FactureAvoirFournisseur");

            migrationBuilder.DropTable(
                name: "FactureAvoirClient");

            migrationBuilder.DropTable(
                name: "SousFamilleProduit");

            migrationBuilder.DropTable(
                name: "InstallationTechnician");

            migrationBuilder.DropTable(
                name: "FactureFournisseur");

            migrationBuilder.DropTable(
                name: "Facture");

            migrationBuilder.DropTable(
                name: "FamilleProduit");

            migrationBuilder.DropTable(
                name: "Fournisseur");

            migrationBuilder.DropTable(
                name: "AccountingYear");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
