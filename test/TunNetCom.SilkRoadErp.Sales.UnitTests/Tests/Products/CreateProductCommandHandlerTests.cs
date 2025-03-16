namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products;

public class CreateProductCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<CreateProductCommandHandler> _testLogger;
    private readonly CreateProductCommandHandler _createProductCommandHandler;
    public CreateProductCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;

        _context = new SalesContext(options);
        _testLogger = new TestLogger<CreateProductCommandHandler>();
        _createProductCommandHandler = new CreateProductCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ProductRefOrNameExists_ReturnsFailResult()
    {
        // Arrange
        var command = new CreateProductCommand(
            Refe: "Refe123New",
            Nom: "Existing Product",
            Qte: 23,
            QteLimite: 22,
            Remise: 20,
            RemiseAchat: 5,
            Tva: 10,
            Prix:56,
            PrixAchat:535,
            Visibilite:true
           );

        var productDuplicated = Produit.CreateProduct(
            refe: "Refe123New",
            nom: "Existing Product",
            qte: 23,
            qteLimite: 22,
            remise: 20,
            remiseAchat: 5,
            tva: 10,
            prix: 56,
            prixAchat: 535,
            visibilite: true);

        _context.Produit.Add(productDuplicated);
        await _context.SaveChangesAsync();

        // Act
        var result = await _createProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("product_refe_or_name_exist", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_NewProduct_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateProductCommand(
            Refe: "RefeABCD",
            Nom: "New Product",
            Qte: 23,
            QteLimite: 22,
            Remise: 20,
            RemiseAchat: 5,
            Tva: 10,
            Prix: 56,
            PrixAchat: 535,
            Visibilite: true);

        // Act
        var result = await _createProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    [Fact]
    public async Task Handle_LogsProductCreated()
    {
        // Arrange
        var command = new CreateProductCommand(
            Refe: "Refe456",
            Nom: "Another new Product",
            Qte: 23,
            QteLimite: 22,
            Remise: 20,
            RemiseAchat: 5,
            Tva: 10,
            Prix: 56,
            PrixAchat: 535,
            Visibilite: true);

        // Act
        await _createProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(
            _testLogger.Logs,
            log => log.Contains($"Creating {nameof(Produit)} with values: {command}"));
    }
    [Fact]
    public async Task Handle_LogsProductCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateProductCommand(
            Refe: "RefeXYZ",
            Nom: "New Product XYZ 4450",
            Qte: 23,
            QteLimite: 22,
            Remise: 20,
            RemiseAchat: 5,
            Tva: 10,
            Prix: 56,
            PrixAchat: 535,
            Visibilite: true);

        // Act
        var result = await _createProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Contains(
            _testLogger.Logs,
            log => log.Contains($"{nameof(Produit)} created successfully with ID: {result.Value}"));
    }

}
