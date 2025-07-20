using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;
public class GetDeliveryNotesBasedOnProductReferenceHandlerTest
{
    private SalesContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SalesContext(options);
    }

    [Fact]
    public async Task Handle_ReturnsFilteredList_WhenProductReferenceMatches()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var fournisseur = Fournisseur.CreateProvider(
            nom: "Fournisseur 1",
            tel: "12345678",
            fax: null,
            matricule: null,
            code: null,
            codeCat: null,
            etbSec: null,
            mail: null,
            mailDeux: null,
            constructeur: true,
            adresse: null);
        fournisseur.Id = 1; 
        var bonReception = new BonDeReception
        {
            Date = new DateTime(2025, 7, 20),
            IdFournisseur = fournisseur.Id,
            IdFournisseurNavigation = fournisseur
        };     
        var ligne = new LigneBonReception
        {
            IdLigne = 1,         
            NumBonRecNavigation = bonReception,
            RefProduit = "REF123",
            DesignationLi = "Produit Test",
            QteLi = 10,
            PrixHt = 100m,
            Remise = 5,
            TotHt = 950,
            Tva = 19,
            TotTtc = 1130.5m
        };     
        context.Fournisseur.Add(fournisseur);
        context.BonDeReception.Add(bonReception);
        context.LigneBonReception.Add(ligne);
        await context.SaveChangesAsync();
        var logger = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)).CreateLogger<GetDeliveryNotesBasedOnProductReferenceHandler>();
        var handler = new GetDeliveryNotesBasedOnProductReferenceHandler(context, logger);
        var query = new GetDeliveryNotesBasedOnProductReferenceQuery("REF123");
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.Single(result);
        var detail = result[0];
        Assert.Equal("REF123", detail.ProductReference);
        Assert.Equal("Produit Test", detail.Description);
        Assert.Equal(10, detail.Quantity);
        Assert.Equal(100m, detail.UnitPriceExcludingTax);
        Assert.Equal("Fournisseur 1", detail.Provider);
        Assert.Equal(new DateTime(2025, 7, 20), detail.Date);
        Assert.True(detail.NetTtcUnitaire > 0);
        Assert.NotNull(detail.PrixHtFodec);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenProductReferenceIsNullOrEmpty()
    {
        var context = CreateInMemoryContext();
        var logger = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)).CreateLogger<GetDeliveryNotesBasedOnProductReferenceHandler>();
        var handler = new GetDeliveryNotesBasedOnProductReferenceHandler(context, logger);
        var emptyQuery = new GetDeliveryNotesBasedOnProductReferenceQuery("");
        var nullQuery = new GetDeliveryNotesBasedOnProductReferenceQuery(null!);
        var whitespaceQuery = new GetDeliveryNotesBasedOnProductReferenceQuery("   ");
        var resultEmpty = await handler.Handle(emptyQuery, CancellationToken.None);
        var resultNull = await handler.Handle(nullQuery, CancellationToken.None);
        var resultWhitespace = await handler.Handle(whitespaceQuery, CancellationToken.None);
        Assert.Empty(resultEmpty);
        Assert.Empty(resultNull);
        Assert.Empty(resultWhitespace);
    }
    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoMatchingProductReference()
    {
        var context = CreateInMemoryContext();
        var logger = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)).CreateLogger<GetDeliveryNotesBasedOnProductReferenceHandler>();
        var handler = new GetDeliveryNotesBasedOnProductReferenceHandler(context, logger);
        var query = new GetDeliveryNotesBasedOnProductReferenceQuery("UNKNOWN_REF");
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.Empty(result);
    }
}
