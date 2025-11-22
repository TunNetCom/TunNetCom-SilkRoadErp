using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                name: "Produit",
                columns: table => new
                {
                    refe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte = table.Column<int>(type: "int", nullable: false),
                    qteLimite = table.Column<int>(type: "int", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    remiseAchat = table.Column<double>(type: "float", nullable: false),
                    TVA = table.Column<double>(type: "float", nullable: false),
                    prix = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    prixAchat = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    visibilite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Produit", x => x.refe);
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
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Systeme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Avoirs",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Avoirs", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.Avoirs_dbo.Client_clientId",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "Id");
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
                    tot_ttc = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
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
                name: "Commandes",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    fournisseurId = table.Column<int>(type: "int", nullable: true)
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
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
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
                name: "LigneAvoirs",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_avoir = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_dbo.LigneAvoirs_dbo.Avoirs_Num_avoir",
                        column: x => x.Num_avoir,
                        principalTable: "Avoirs",
                        principalColumn: "Num");
                    table.ForeignKey(
                        name: "FK_dbo.LigneAvoirs_dbo.Produit_Ref_Produit",
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
                    Num_devis = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_dbo.LigneDevis_dbo.Devis_Num_devis",
                        column: x => x.Num_devis,
                        principalTable: "Devis",
                        principalColumn: "Num");
                    table.ForeignKey(
                        name: "FK_dbo.LigneDevis_dbo.Produit_Ref_produit",
                        column: x => x.Ref_produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
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
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.BonDeLivraison", x => x.Id);
                    table.UniqueConstraint("AK_BonDeLivraison_Num", x => x.Num);
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
                    AccountingYearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.BonDeReception", x => x.Id);
                    table.UniqueConstraint("AK_BonDeReception_Num", x => x.Num);
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
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_FactureAvoirFourSurPAge = table.Column<int>(type: "int", nullable: false),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Num_FactureFournisseur = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.FactureAvoirFournisseur", x => x.Num);
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
                name: "LigneBL",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_BL = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_dbo.LigneBL", x => x.Id_li);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBL_dbo.BonDeLivraison_Num_BL",
                        column: x => x.Num_BL,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBL_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Num_BL = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    date_tr = table.Column<DateTime>(type: "datetime", nullable: false),
                    montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.Transaction", x => x.Num_BL);
                    table.ForeignKey(
                        name: "FK_dbo.Transaction_dbo.BonDeLivraison_Num_BL",
                        column: x => x.Num_BL,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Num");
                });

            migrationBuilder.CreateTable(
                name: "LigneBonReception",
                columns: table => new
                {
                    Id_ligne = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_BonRec = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_dbo.LigneBonReception_dbo.BonDeReception_Num_BonRec",
                        column: x => x.Num_BonRec,
                        principalTable: "BonDeReception",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.LigneBonReception_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            migrationBuilder.CreateTable(
                name: "AvoirFournisseur",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    fournisseurId = table.Column<int>(type: "int", nullable: true),
                    Num_FactureAvoirFournisseur = table.Column<int>(type: "int", nullable: true),
                    Num_AvoirFournisseur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.AvoirFournisseur", x => x.Num);
                    table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_Num_FactureAvoirFournisseur",
                        column: x => x.Num_FactureAvoirFournisseur,
                        principalTable: "FactureAvoirFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.Fournisseur_fournisseurId",
                        column: x => x.fournisseurId,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "LigneAvoirFournisseur",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_AvoirFr = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_dbo.LigneAvoirFournisseur_dbo.AvoirFournisseur_Num_AvoirFr",
                        column: x => x.Num_AvoirFr,
                        principalTable: "AvoirFournisseur",
                        principalColumn: "Num",
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
                name: "IX_AvoirFournisseur_fournisseurId",
                table: "AvoirFournisseur",
                column: "fournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur",
                column: "Num_FactureAvoirFournisseur");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_clientId",
                table: "Avoirs",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_AccountingYearId",
                table: "BonDeLivraison",
                column: "AccountingYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_clientId",
                table: "BonDeLivraison",
                column: "clientId");

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
                name: "IX_FactureAvoirFournisseur_id_fournisseur",
                table: "FactureAvoirFournisseur",
                column: "id_fournisseur");

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
                name: "IX_LigneAvoirFournisseur_Num_AvoirFr",
                table: "LigneAvoirFournisseur",
                column: "Num_AvoirFr");

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirFournisseur_Ref_Produit",
                table: "LigneAvoirFournisseur",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirs_Num_avoir",
                table: "LigneAvoirs",
                column: "Num_avoir");

            migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirs_Ref_Produit",
                table: "LigneAvoirs",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBL_Num_BL",
                table: "LigneBL",
                column: "Num_BL");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBL_Ref_Produit",
                table: "LigneBL",
                column: "Ref_Produit");

            migrationBuilder.CreateIndex(
                name: "IX_LigneBonReception_Num_BonRec",
                table: "LigneBonReception",
                column: "Num_BonRec");

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
                name: "IX_LigneDevis_Num_devis",
                table: "LigneDevis",
                column: "Num_devis");

            migrationBuilder.CreateIndex(
                name: "IX_LigneDevis_Ref_produit",
                table: "LigneDevis",
                column: "Ref_produit");

            // Create SQL Views
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.ReceiptNoteView', 'V') IS NOT NULL
    DROP VIEW [dbo].[ReceiptNoteView];
");

            migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ReceiptNoteView]
AS
SELECT dbo.BonDeReception.Num, dbo.BonDeReception.date AS Date, SUM(dbo.LigneBonReception.tot_TTC) AS TotalTTC, dbo.BonDeReception.Num_Bon_fournisseur AS NumBonFournisseur, 
                  dbo.BonDeReception.date_livraison AS DateLivraison, dbo.BonDeReception.id_fournisseur AS IdFournisseur, dbo.BonDeReception.Num_Facture_fournisseur AS NumFactureFournisseur, SUM(dbo.LigneBonReception.tot_HT) 
                  AS TotHt
FROM     dbo.BonDeReception INNER JOIN
                  dbo.LigneBonReception ON dbo.BonDeReception.Num = dbo.LigneBonReception.Num_BonRec
GROUP BY dbo.BonDeReception.Num, dbo.BonDeReception.date, dbo.BonDeReception.Num_Bon_fournisseur, dbo.BonDeReception.date_livraison, dbo.BonDeReception.id_fournisseur, dbo.BonDeReception.Num_Facture_fournisseur;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.ProviderInvoiceView', 'V') IS NOT NULL
    DROP VIEW [dbo].[ProviderInvoiceView];
");

            migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ProviderInvoiceView]
AS
SELECT dbo.FactureFournisseur.Num, dbo.FactureFournisseur.id_fournisseur AS ProviderId, dbo.FactureFournisseur.NumFactureFournisseur AS ProviderInvoiceNumber, dbo.FactureFournisseur.dateFacturationFournisseur AS InvoicingDate, 
                  dbo.FactureFournisseur.date AS Date, SUM(dbo.LigneBonReception.tot_HT) AS TotalHT, SUM(dbo.LigneBonReception.tot_TTC) AS TotalTTC
FROM     dbo.FactureFournisseur INNER JOIN
                  dbo.BonDeReception ON dbo.FactureFournisseur.Num = dbo.BonDeReception.Num_Facture_fournisseur INNER JOIN
                  dbo.LigneBonReception ON dbo.BonDeReception.Num = dbo.LigneBonReception.Num_BonRec
GROUP BY dbo.FactureFournisseur.Num, dbo.FactureFournisseur.id_fournisseur, dbo.FactureFournisseur.NumFactureFournisseur, dbo.FactureFournisseur.dateFacturationFournisseur, dbo.FactureFournisseur.date;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvoirFinancierFournisseurs");

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
                name: "Systeme");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "AvoirFournisseur");

            migrationBuilder.DropTable(
                name: "Avoirs");

            migrationBuilder.DropTable(
                name: "BonDeReception");

            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropTable(
                name: "Devis");

            migrationBuilder.DropTable(
                name: "Produit");

            migrationBuilder.DropTable(
                name: "BonDeLivraison");

            migrationBuilder.DropTable(
                name: "FactureAvoirFournisseur");

            migrationBuilder.DropTable(
                name: "Facture");

            migrationBuilder.DropTable(
                name: "FactureFournisseur");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "AccountingYear");

            migrationBuilder.DropTable(
                name: "Fournisseur");

            // Drop SQL Views
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[ReceiptNoteView]");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[ProviderInvoiceView]");
        }
    }
}
