namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products;

public class GetProductByRefQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetProductByRefQueryHandler> _testLogger;
    private readonly GetProductByRefQueryHandler _getProductByRefQueryHandler;

    public GetProductByRefQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;

        _context = new SalesContext(options);
        _testLogger = new TestLogger<GetProductByRefQueryHandler>();
        _getProductByRefQueryHandler = new GetProductByRefQueryHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsCustomer()
    {
        // Arrange
        var product = Produit.CreateProduct
            (
            refe: "Refetest1",
            nom: "test Product",
            qte: 23,
            qteLimite: 22,
            remise: 20,
            remiseAchat: 5,
            tva: 10,
            prix: 56,
            prixAchat: 535,
            visibilite: true);

        _context.Produit.Add(product);
        await _context.SaveChangesAsync();

        var query = new GetProductByRefQuery(product.Refe);

        // Act
        var result = await _getProductByRefQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Nom, result.Value.Nom);
    }

    [Fact]
    public async Task Handle_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidRef = "test2";
        var query = new GetProductByRefQuery(invalidRef);

        // Act
        var result = await _getProductByRefQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Product_not_found", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_LogsProducttNotFound()
    {
        // Arrange
        var invalidRef = "test2";
        var query = new GetProductByRefQuery(invalidRef);

        // Act
        var result = await _getProductByRefQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"product with Refe: {query.Refe} not found"));
    }
}
