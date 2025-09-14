public class GetProductByRefQueryHandlerTest
{
    private readonly SalesContext _context;
    private readonly Mock<ILogger<GetProductByRefQueryHandler>> _loggerMock;
    private readonly GetProductByRefQueryHandler _handler;
    public GetProductByRefQueryHandlerTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new SalesContext(options);
        _loggerMock = new Mock<ILogger<GetProductByRefQueryHandler>>();
        _handler = new GetProductByRefQueryHandler(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = Produit.CreateProduct(
            refe: "P001",
            nom: "Produit Test",
            qte: 10,
            qteLimite: 5,
            remise: 5,
            remiseAchat: 2,
            tva: 19,
            prix: 100m,
            prixAchat: 80m,
            visibilite: true);
        _ = await _context.Produit.AddAsync(product);
        _ = await _context.SaveChangesAsync();
        var query = new GetProductByRefQuery("P001");
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("P001", result.Value.Reference);
        Assert.Equal("Produit Test", result.Value.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProductNotFound()
    {
        // Arrange
        var query = new GetProductByRefQuery("NOT_EXIST");
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "not_found");
    }
}
