namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers;

public class GetProviderByIdQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetProviderByIdQueryHandler> _testLogger;
    private readonly GetProviderByIdQueryHandler _handler;

    public GetProviderByIdQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
            _context = new SalesContext(options);
            _testLogger = new TestLogger<GetProviderByIdQueryHandler>();
            _handler = new GetProviderByIdQueryHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ProviderNotFound_ReturnsFailResult()
    {
        // Arrange
        var query = new GetProviderByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("provider_not_found", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ProviderFound_ReturnsProvider()
    {
        // Arrange
        var provider = Fournisseur.CreateProvider(
             nom: "Provider",
             tel: "123456789",
             fax: "Fax",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "etbsec",
             mail: "email@example.com",
             mailDeux: "email@example.com",
             constructeur: true,
             adresse: "adresse");
        _context.Fournisseur.Add(provider);
        await _context.SaveChangesAsync();

        var query = new GetProviderByIdQuery(provider.Id);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(provider.Nom, result.Value.Nom);
    }

    [Fact]
    public async Task Handle_LogsProviderNotFound()
    {
        // Arrange
        var query = new GetProviderByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("provider_not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fournisseur with ID: {query.Id} not found"));
    }
}