using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
public class GetFullOrderQueryHandlerTest
{
    private DbContextOptions<SalesContext> CreateInMemoryDbOptions() =>
        new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCommandeDoesNotExist()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        await using var context = new SalesContext(options);
        var loggerMock = new Mock<ILogger<GetFullOrderQueryHandler>>();
        var handler = new GetFullOrderQueryHandler(context, loggerMock.Object);
        var query = new GetFullOrderQuery(999); 
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
       // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is EntityNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFullOrder_WhenCommandeExists()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        await using var context = new SalesContext(options);
        var fournisseur = new Fournisseur
        {
            Id = 1,
            Nom = "Fournisseur Test", 
            Tel = "12345678",
            Adresse = "Tunis",
            Matricule = "MF123",
            Code = "F001",
            CodeCat = "CAT01",
            EtbSec = "Yes",
            Mail = "fournisseur@mail.com"
        };
        var commande = new Commandes
        {
            Num = 1,
            Date = new DateTime(2024, 1, 1),
            FournisseurId = 1,
            Fournisseur = fournisseur,
            LigneCommandes = new List<LigneCommandes>
    {
        new LigneCommandes
        {
            IdLi = 1,
            NumCommande = 1,
            RefProduit = "PROD1",
            DesignationLi = "Produit 1",
            QteLi = 2,
            PrixHt = 10,
            TotHt = 20,
            TotTtc = 24,
            Tva = 20,
            Remise = 0
        }
    }
        };
        await context.Fournisseur.AddAsync(fournisseur);
        await context.Commandes.AddAsync(commande);
        await context.SaveChangesAsync();
        var loggerMock = new Mock<ILogger<GetFullOrderQueryHandler>>();
        var handler = new GetFullOrderQueryHandler(context, loggerMock.Object);
        var query = new GetFullOrderQuery(1);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        var response = result.Value;
        Assert.Equal(1, response.OrderNumber);
        Assert.Equal(20, response.TotalExcludingVat);
        Assert.Equal(24, response.NetToPay);
        Assert.Equal(4, response.TotalVat);
        Assert.Single(response.OrderLines);
        Assert.Equal("Fournisseur Test", response.Supplier.Name);
    }
}
