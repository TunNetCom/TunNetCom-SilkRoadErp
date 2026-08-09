using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Services;

public class StockCalculationServiceTest
{
    private const int AccountingYearId = 2024;

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"StockCalculation_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static StockCalculationService CreateService(SalesContext context)
    {
        return new StockCalculationService(context, new TestLogger<StockCalculationService>());
    }

    private static Produit CreateProduit(string refe = "PRD-001")
    {
        return new Produit(
            refe: refe,
            nom: "Produit",
            qteLimite: 10,
            remise: 0,
            remiseAchat: 0,
            tva: 19,
            prix: 100m,
            prixAchat: 80m,
            visibilite: true);
    }

    [Fact]
    public async Task CalculateStockAsync_WhenProductNotFound_ShouldReturnZeroedResult()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CalculateStockAsync("INEXISTANT", AccountingYearId, CancellationToken.None);

        result.Reference.Should().Be("INEXISTANT");
        result.StockInitial.Should().Be(0);
        result.TotalAchats.Should().Be(0);
        result.TotalVentes.Should().Be(0);
        result.TotalAvoirsClients.Should().Be(0);
        result.StockCalcule.Should().Be(0);
        result.StockDisponible.Should().Be(0);
        result.QteEnRetourFournisseur.Should().Be(0);
        result.QteEnReparation.Should().Be(0);
        result.QteEnAttenteReception.Should().Be(0);
        result.StockReel.Should().Be(0);
    }

    [Fact]
    public async Task CalculateStockAsync_WithInventoryPurchasesSalesAvoirs_ShouldComputeTotals()
    {
        using var context = CreateContext();
        _ = context.Produit.Add(CreateProduit());

        var inventaire = Inventaire.CreateInventaire(1, AccountingYearId, DateTime.Now, null);
        inventaire.Valider();
        _ = context.Inventaire.Add(inventaire);
        _ = context.SaveChanges();
        _ = context.LigneInventaire.Add(LigneInventaire.CreateLigneInventaire(
            inventaireId: inventaire.Id, refProduit: "PRD-001", quantiteTheorique: 20, quantiteReelle: 15,
            prixHt: 100m, dernierPrixAchat: 80m));

        var bonReception = BonDeReception.CreateReceiptNote(
            num: 1, numBonFournisseur: 1, dateLivraison: DateTime.Now, idFournisseur: 1,
            date: DateTime.Now, numFactureFournisseur: null, accountingYearId: AccountingYearId,
            totHTva: 100, totTva: 19, netPayer: 119);
        _ = context.BonDeReception.Add(bonReception);
        _ = context.SaveChanges();
        _ = context.LigneBonReception.Add(new LigneBonReception
        {
            BonDeReceptionId = bonReception.Id,
            RefProduit = "PRD-001",
            DesignationLi = "P",
            QteLi = 10,
            PrixHt = 10m,
            Remise = 0,
            TotHt = 100m,
            Tva = 19,
            TotTtc = 119m
        });

        var bonLivraison = new BonDeLivraison
        {
            Num = 1,
            Date = DateTime.Now,
            TotHTva = 100m,
            TotTva = 19m,
            NetPayer = 119m,
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = AccountingYearId
        };
        _ = context.BonDeLivraison.Add(bonLivraison);
        _ = context.SaveChanges();
        _ = context.LigneBl.Add(new LigneBl
        {
            BonDeLivraisonId = bonLivraison.Id,
            RefProduit = "PRD-001",
            DesignationLi = "P",
            QteLi = 4,
            PrixHt = 10m,
            Remise = 0,
            TotHt = 40m,
            Tva = 19,
            TotTtc = 47.6m
        });

        var avoir = Avoirs.CreateAvoir(DateTime.Now, clientId: 1, accountingYearId: AccountingYearId);
        _ = context.Avoirs.Add(avoir);
        _ = context.SaveChanges();
        _ = context.LigneAvoirs.Add(new LigneAvoirs
        {
            AvoirsId = avoir.Id,
            RefProduit = "PRD-001",
            DesignationLi = "P",
            QteLi = 2,
            PrixHt = 10m,
            Remise = 0,
            TotHt = 20m,
            Tva = 19,
            TotTtc = 23.8m
        });
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.CalculateStockAsync("PRD-001", AccountingYearId, CancellationToken.None);

        result.Reference.Should().Be("PRD-001");
        result.StockInitial.Should().Be(15);
        result.TotalAchats.Should().Be(10);
        result.TotalVentes.Should().Be(4);
        result.TotalAvoirsClients.Should().Be(2);
        result.StockCalcule.Should().Be(15 + 10 - 4 + 2);
        result.StockDisponible.Should().Be(23);
        result.StockReel.Should().Be(23);
    }

    [Fact]
    public async Task CalculateStockAsync_WhenValidRetourFournisseur_ShouldExcludeFromAvailable()
    {
        using var context = CreateContext();
        _ = context.Produit.Add(CreateProduit());

        var retour = RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur(
            num: 1, date: DateTime.Now, idFournisseur: 1, accountingYearId: AccountingYearId,
            totHTva: 100, totTva: 19, netPayer: 119);
        retour.Valider();
        _ = context.RetourMarchandiseFournisseur.Add(retour);
        _ = context.SaveChanges();
        _ = context.LigneRetourMarchandiseFournisseur.Add(LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: retour.Id, productRef: "PRD-001", designationLigne: "P",
            quantity: 10, unitPrice: 10m, discount: 0, tax: 19, qteRecue: 4));
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.CalculateStockAsync("PRD-001", AccountingYearId, CancellationToken.None);

        result.QteEnRetourFournisseur.Should().Be(10);
        result.QteEnReparation.Should().Be(6); // 10 - 4 chez le fournisseur
        result.QteEnAttenteReception.Should().Be(0); // statut Valid, pas reception partielle
        result.StockDisponible.Should().Be(0);
        result.StockReel.Should().Be(-6);
    }

    [Fact]
    public async Task CalculateStockAsync_WhenReceptionPartielle_ShouldComputeAttenteReception()
    {
        using var context = CreateContext();
        _ = context.Produit.Add(CreateProduit());

        var retour = RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur(
            num: 1, date: DateTime.Now, idFournisseur: 1, accountingYearId: AccountingYearId,
            totHTva: 100, totTva: 19, netPayer: 119);
        retour.Valider();
        retour.PasserEnReparation();
        _ = context.RetourMarchandiseFournisseur.Add(retour);
        _ = context.SaveChanges();
        _ = context.LigneRetourMarchandiseFournisseur.Add(LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: retour.Id, productRef: "PRD-001", designationLigne: "P",
            quantity: 8, unitPrice: 10m, discount: 0, tax: 19, qteRecue: 3));
        retour.ValiderReception(); // 3 recue sur 8 => reception partielle
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.CalculateStockAsync("PRD-001", AccountingYearId, CancellationToken.None);

        result.QteEnRetourFournisseur.Should().Be(8);
        result.QteEnReparation.Should().Be(5);
        result.QteEnAttenteReception.Should().Be(5);
    }

    [Fact]
    public async Task CalculateStockAsync_WhenOnlyDraftRetour_ShouldIgnoreForStock()
    {
        using var context = CreateContext();
        _ = context.Produit.Add(CreateProduit());

        var retour = RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur(
            num: 1, date: DateTime.Now, idFournisseur: 1, accountingYearId: AccountingYearId,
            totHTva: 100, totTva: 19, netPayer: 119);
        _ = context.RetourMarchandiseFournisseur.Add(retour);
        _ = context.SaveChanges();
        _ = context.LigneRetourMarchandiseFournisseur.Add(LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: retour.Id, productRef: "PRD-001", designationLigne: "P",
            quantity: 10, unitPrice: 10m, discount: 0, tax: 19));
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.CalculateStockAsync("PRD-001", AccountingYearId, CancellationToken.None);

        result.QteEnRetourFournisseur.Should().Be(0);
        result.QteEnReparation.Should().Be(0);
        result.QteEnAttenteReception.Should().Be(0);
    }

    [Fact]
    public async Task CalculateStocksAsync_WithMultipleProducts_ShouldReturnDictionary()
    {
        using var context = CreateContext();
        _ = context.Produit.Add(CreateProduit("PRD-A"));
        _ = context.Produit.Add(CreateProduit("PRD-B"));

        var inventaire = Inventaire.CreateInventaire(1, AccountingYearId, DateTime.Now, null);
        inventaire.Valider();
        _ = context.Inventaire.Add(inventaire);
        _ = context.SaveChanges();
        _ = context.LigneInventaire.Add(LigneInventaire.CreateLigneInventaire(
            inventaireId: inventaire.Id, refProduit: "PRD-A", quantiteTheorique: 10, quantiteReelle: 10,
            prixHt: 100m, dernierPrixAchat: 80m));
        _ = context.SaveChanges();

        var service = CreateService(context);
        var results = await service.CalculateStocksAsync(
            new List<string> { "PRD-A", "PRD-B" }, AccountingYearId, CancellationToken.None);

        results.Should().HaveCount(2);
        results["PRD-A"].StockInitial.Should().Be(10);
        results["PRD-A"].StockCalcule.Should().Be(10);
        results["PRD-B"].StockInitial.Should().Be(0);
        results["PRD-B"].StockCalcule.Should().Be(0);
    }

    [Fact]
    public async Task CalculateStocksAsync_WhenProductMissing_ShouldStillReturnZeroedEntry()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var results = await service.CalculateStocksAsync(
            new List<string> { "MISSING" }, AccountingYearId, CancellationToken.None);

        results.Should().HaveCount(1);
        var result = results["MISSING"];
        result.StockInitial.Should().Be(0);
        result.TotalAchats.Should().Be(0);
        result.TotalVentes.Should().Be(0);
        result.StockCalcule.Should().Be(0);
    }
}
