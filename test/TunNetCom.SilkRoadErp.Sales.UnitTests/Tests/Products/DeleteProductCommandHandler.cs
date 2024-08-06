namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<DeleteProductCommandHandler> _testLogger;
    private readonly DeleteProductCommandHandler _deleteProductCommandHandler;

    public DeleteProductCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;

        _context = new SalesContext(options);
        _testLogger = new TestLogger<DeleteProductCommandHandler>();
        _deleteProductCommandHandler = new DeleteProductCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnError()
    {
        // Arrange
        var command = new DeleteProductCommand(Refe: "test1");

        // Act
        var result = await _deleteProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("product_not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Product with ID: {command.Refe} not found"));
    }

    [Fact]
    public async Task Handle_ProductDeleted_ReturnSuccess()
    {
        // Arrange
        var product = Produit.CreateProduct(
           refe: "Refe123",
            nom: "Existing Product",
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

        var command = new DeleteProductCommand(Refe: product.Refe);

        // Act
        var result = await _deleteProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_context.Produit, p => p.Refe == product.Refe);
    }

    [Fact]
    public async Task Handle_LogsProductDeleted()
    {
        // Arrange
        var product = Produit.CreateProduct(
             refe: "Refe123",
            nom: "Existing Product",
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

        var command = new DeleteProductCommand(Refe: product.Refe);

        // Act
        var result = await _deleteProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Product with ID: Refe123 deleted successfully"));
    
    }
}
