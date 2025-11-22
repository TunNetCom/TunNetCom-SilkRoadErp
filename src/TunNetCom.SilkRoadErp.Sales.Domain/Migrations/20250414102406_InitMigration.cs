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
            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.Client", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.Fournisseur", x => x.id);
                });

            _ = migrationBuilder.CreateTable(
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
                    prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    prixAchat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    visibilite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.Produit", x => x.refe);
                });

            _ = migrationBuilder.CreateTable(
                name: "ProviderInvoiceView",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    ProviderInvoiceNumber = table.Column<long>(type: "bigint", nullable: false),
                    InvoicingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalTTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalHT = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });

            _ = migrationBuilder.CreateTable(
                name: "ReceiptNoteView",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false),
                    NumBonFournisseur = table.Column<long>(type: "bigint", nullable: false),
                    DateLivraison = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdFournisseur = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumFactureFournisseur = table.Column<int>(type: "int", nullable: true),
                    TotalTTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotHt = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });

            _ = migrationBuilder.CreateTable(
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
                    pourcentageRetenu = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.Systeme", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.Avoirs", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.Avoirs_dbo.Client_clientId",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.Devis", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.Devis_dbo.Client_id_client",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "Facture",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_client = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.Facture", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.Facture_dbo.Client_id_client",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.Commandes", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.Commandes_dbo.Fournisseur_fournisseurId",
                        column: x => x.fournisseurId,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.EcheanceDesFournisseurs", x => x.id);
                    _ = table.ForeignKey(
                        name: "FK_dbo.EcheanceDesFournisseurs_dbo.Fournisseur_fournisseur_id",
                        column: x => x.fournisseur_id,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            _ = migrationBuilder.CreateTable(
                name: "FactureFournisseur",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    paye = table.Column<bool>(type: "bit", nullable: false),
                    NumFactureFournisseur = table.Column<long>(type: "bigint", nullable: false),
                    dateFacturationFournisseur = table.Column<DateTime>(type: "datetime", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.FactureFournisseur", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.FactureFournisseur_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            _ = migrationBuilder.CreateTable(
                name: "LigneAvoirs",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_avoir = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.LigneAvoirs", x => x.Id_li);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneAvoirs_dbo.Avoirs_Num_avoir",
                        column: x => x.Num_avoir,
                        principalTable: "Avoirs",
                        principalColumn: "Num");
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneAvoirs_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            _ = migrationBuilder.CreateTable(
                name: "LigneDevis",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_devis = table.Column<int>(type: "int", nullable: false),
                    Ref_produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.LigneDevis", x => x.Id_li);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneDevis_dbo.Devis_Num_devis",
                        column: x => x.Num_devis,
                        principalTable: "Devis",
                        principalColumn: "Num");
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneDevis_dbo.Produit_Ref_produit",
                        column: x => x.Ref_produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            _ = migrationBuilder.CreateTable(
                name: "BonDeLivraison",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    tot_H_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tot_tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    net_payer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    temp_bl = table.Column<TimeOnly>(type: "time", nullable: false),
                    Num_Facture = table.Column<int>(type: "int", nullable: true),
                    clientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.BonDeLivraison", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.BonDeLivraison_dbo.Client_clientId",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                    _ = table.ForeignKey(
                        name: "FK_dbo.BonDeLivraison_dbo.Facture_Num_Facture",
                        column: x => x.Num_Facture,
                        principalTable: "Facture",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "LigneCommandes",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_commande = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.LigneCommandes", x => x.Id_li);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneCommandes_dbo.Commandes_Num_commande",
                        column: x => x.Num_commande,
                        principalTable: "Commandes",
                        principalColumn: "Num");
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneCommandes_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.AvoirFinancierFournisseurs", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_Num",
                        column: x => x.Num,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "BonDeReception",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_Bon_fournisseur = table.Column<long>(type: "bigint", nullable: false),
                    date_livraison = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_fournisseur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Num_Facture_fournisseur = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.BonDeReception", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.BonDeReception_dbo.FactureFournisseur_Num_Facture_fournisseur",
                        column: x => x.Num_Facture_fournisseur,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_dbo.BonDeReception_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.FactureAvoirFournisseur", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_Num_FactureFournisseur",
                        column: x => x.Num_FactureFournisseur,
                        principalTable: "FactureFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_dbo.FactureAvoirFournisseur_dbo.Fournisseur_id_fournisseur",
                        column: x => x.id_fournisseur,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            _ = migrationBuilder.CreateTable(
                name: "LigneBL",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_BL = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.LigneBL", x => x.Id_li);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneBL_dbo.BonDeLivraison_Num_BL",
                        column: x => x.Num_BL,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneBL_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.Transaction", x => x.Num_BL);
                    _ = table.ForeignKey(
                        name: "FK_dbo.Transaction_dbo.BonDeLivraison_Num_BL",
                        column: x => x.Num_BL,
                        principalTable: "BonDeLivraison",
                        principalColumn: "Num");
                });

            _ = migrationBuilder.CreateTable(
                name: "LigneBonReception",
                columns: table => new
                {
                    Id_ligne = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_BonRec = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.LigneBonReception", x => x.Id_ligne);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneBonReception_dbo.BonDeReception_Num_BonRec",
                        column: x => x.Num_BonRec,
                        principalTable: "BonDeReception",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneBonReception_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_dbo.AvoirFournisseur", x => x.Num);
                    _ = table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_Num_FactureAvoirFournisseur",
                        column: x => x.Num_FactureAvoirFournisseur,
                        principalTable: "FactureAvoirFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_dbo.AvoirFournisseur_dbo.Fournisseur_fournisseurId",
                        column: x => x.fournisseurId,
                        principalTable: "Fournisseur",
                        principalColumn: "id");
                });

            _ = migrationBuilder.CreateTable(
                name: "LigneAvoirFournisseur",
                columns: table => new
                {
                    Id_li = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num_AvoirFr = table.Column<int>(type: "int", nullable: false),
                    Ref_Produit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    designation_li = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qte_li = table.Column<int>(type: "int", nullable: false),
                    prix_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remise = table.Column<double>(type: "float", nullable: false),
                    tot_HT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tva = table.Column<double>(type: "float", nullable: false),
                    tot_TTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbo.LigneAvoirFournisseur", x => x.Id_li);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneAvoirFournisseur_dbo.AvoirFournisseur_Num_AvoirFr",
                        column: x => x.Num_AvoirFr,
                        principalTable: "AvoirFournisseur",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_dbo.LigneAvoirFournisseur_dbo.Produit_Ref_Produit",
                        column: x => x.Ref_Produit,
                        principalTable: "Produit",
                        principalColumn: "refe");
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_fournisseurId",
                table: "AvoirFournisseur",
                column: "fournisseurId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseur_Num_FactureAvoirFournisseur",
                table: "AvoirFournisseur",
                column: "Num_FactureAvoirFournisseur");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Avoirs_clientId",
                table: "Avoirs",
                column: "clientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_clientId",
                table: "BonDeLivraison",
                column: "clientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_BonDeLivraison_Num_Facture",
                table: "BonDeLivraison",
                column: "Num_Facture");

            _ = migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_id_fournisseur",
                table: "BonDeReception",
                column: "id_fournisseur");

            _ = migrationBuilder.CreateIndex(
                name: "IX_BonDeReception_Num_Facture_fournisseur",
                table: "BonDeReception",
                column: "Num_Facture_fournisseur");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Commandes_fournisseurId",
                table: "Commandes",
                column: "fournisseurId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Devis_id_client",
                table: "Devis",
                column: "id_client");

            _ = migrationBuilder.CreateIndex(
                name: "IX_EcheanceDesFournisseurs_fournisseur_id",
                table: "EcheanceDesFournisseurs",
                column: "fournisseur_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Facture_id_client",
                table: "Facture",
                column: "id_client");

            _ = migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_id_fournisseur",
                table: "FactureAvoirFournisseur",
                column: "id_fournisseur");

            _ = migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirFournisseur_Num_FactureFournisseur",
                table: "FactureAvoirFournisseur",
                column: "Num_FactureFournisseur");

            _ = migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseur_id_fournisseur",
                table: "FactureFournisseur",
                column: "id_fournisseur");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirFournisseur_Num_AvoirFr",
                table: "LigneAvoirFournisseur",
                column: "Num_AvoirFr");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirFournisseur_Ref_Produit",
                table: "LigneAvoirFournisseur",
                column: "Ref_Produit");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirs_Num_avoir",
                table: "LigneAvoirs",
                column: "Num_avoir");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneAvoirs_Ref_Produit",
                table: "LigneAvoirs",
                column: "Ref_Produit");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneBL_Num_BL",
                table: "LigneBL",
                column: "Num_BL");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneBL_Ref_Produit",
                table: "LigneBL",
                column: "Ref_Produit");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneBonReception_Num_BonRec",
                table: "LigneBonReception",
                column: "Num_BonRec");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneBonReception_Ref_Produit",
                table: "LigneBonReception",
                column: "Ref_Produit");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneCommandes_Num_commande",
                table: "LigneCommandes",
                column: "Num_commande");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneCommandes_Ref_Produit",
                table: "LigneCommandes",
                column: "Ref_Produit");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneDevis_Num_devis",
                table: "LigneDevis",
                column: "Num_devis");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LigneDevis_Ref_produit",
                table: "LigneDevis",
                column: "Ref_produit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "AvoirFinancierFournisseurs");

            _ = migrationBuilder.DropTable(
                name: "EcheanceDesFournisseurs");

            _ = migrationBuilder.DropTable(
                name: "LigneAvoirFournisseur");

            _ = migrationBuilder.DropTable(
                name: "LigneAvoirs");

            _ = migrationBuilder.DropTable(
                name: "LigneBL");

            _ = migrationBuilder.DropTable(
                name: "LigneBonReception");

            _ = migrationBuilder.DropTable(
                name: "LigneCommandes");

            _ = migrationBuilder.DropTable(
                name: "LigneDevis");

            _ = migrationBuilder.DropTable(
                name: "ProviderInvoiceView");

            _ = migrationBuilder.DropTable(
                name: "ReceiptNoteView");

            _ = migrationBuilder.DropTable(
                name: "Systeme");

            _ = migrationBuilder.DropTable(
                name: "Transaction");

            _ = migrationBuilder.DropTable(
                name: "AvoirFournisseur");

            _ = migrationBuilder.DropTable(
                name: "Avoirs");

            _ = migrationBuilder.DropTable(
                name: "BonDeReception");

            _ = migrationBuilder.DropTable(
                name: "Commandes");

            _ = migrationBuilder.DropTable(
                name: "Devis");

            _ = migrationBuilder.DropTable(
                name: "Produit");

            _ = migrationBuilder.DropTable(
                name: "BonDeLivraison");

            _ = migrationBuilder.DropTable(
                name: "FactureAvoirFournisseur");

            _ = migrationBuilder.DropTable(
                name: "Facture");

            _ = migrationBuilder.DropTable(
                name: "FactureFournisseur");

            _ = migrationBuilder.DropTable(
                name: "Client");

            _ = migrationBuilder.DropTable(
                name: "Fournisseur");
        }
    }
}
