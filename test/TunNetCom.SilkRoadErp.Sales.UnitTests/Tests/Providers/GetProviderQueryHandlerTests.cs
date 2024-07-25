namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers;

public class GetProviderQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetProviderQueryHandler> _testLogger;
    private readonly GetProviderQueryHandler _handler;

    public GetProviderQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options; 
            _context = new SalesContext(options);
            _testLogger = new TestLogger<GetProviderQueryHandler>();
            _handler = new GetProviderQueryHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_PaginationRequest_LogsPagination()
    {
        // Arrange
        var query = new GetProviderQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: null
        );

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fetching Fournisseur with pageIndex: {query.PageNumber} and pageSize: {query.PageSize}"));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_SearchKeyword_FiltersProviders()
    {
        //Arrange
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

        var query = new GetProviderQuery(
          PageNumber: 1,
          PageSize: 10,
          SearchKeyword: "Provider"
      );

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Provider", result.First().Nom);
    }

    [Fact]
    public async Task Handle_EmptySearchKeyword_ReturnsAllProviders()
    {
        //Arrange
        var provider1 = Fournisseur.CreateProvider(
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

        var provider2 = Fournisseur.CreateProvider(
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

        _context.Fournisseur.Add(provider2);
        _context.Fournisseur.Add(provider1);
        await _context.SaveChangesAsync();

        var query = new GetProviderQuery(
           PageNumber: 1,
           PageSize: 10,
           SearchKeyword: ""
       );

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
