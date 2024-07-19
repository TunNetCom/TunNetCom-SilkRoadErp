namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers;

public class GetCustomerQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetCustomerQueryHandler> _testLogger;
    private readonly GetCustomerQueryHandler _getCustomerQueryHandler;

    public GetCustomerQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;

        _context = new SalesContext(options);
        _testLogger = new TestLogger<GetCustomerQueryHandler>();
        _getCustomerQueryHandler = new GetCustomerQueryHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_PaginationRequest_LogsPagination()
    {
        // Arrange
        var query = new GetCustomerQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: null
        );

        // Act
        var result = await _getCustomerQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fetching customers with pageIndex: {query.PageNumber} and pageSize: {query.PageSize}"));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_SearchKeyword_FiltersCustomers()
    {
        // Arrange
        var client = Client.CreateClient
            (
             nom: "Client tojrab",
             tel: "1234567898",
             adresse: "houni much baid",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "EtbSec",
             mail: "email@example.com");

        _context.Client.Add(client);
        await _context.SaveChangesAsync();

        var query = new GetCustomerQuery(
           PageNumber: 1,
           PageSize: 10,
           SearchKeyword: "Client tojrab"
       );

        // Act
        var result = await _getCustomerQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Client tojrab", result.First().Nom);
    }

    [Fact]
    public async Task Handle_EmptySearchKeyword_ReturnsAllCustomers()
    {
        // Arrange
        var client1 = Client.CreateClient
            (
             nom: "Client tojrab1",
             tel: "1234567898",
             adresse: "houni much baid",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "EtbSec",
             mail: "email@example.com");

        var client2 = Client.CreateClient
            (
             nom: "Client tojrab2",
             tel: "1234567898",
             adresse: "houni much baid",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "EtbSec",
             mail: "email@example.com");

        var client3 = Client.CreateClient
            (
             nom: "Client tojrab3",
             tel: "1234567898",
             adresse: "houni much baid",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "EtbSec",
             mail: "email@example.com");
        _context.Client.AddRange(client1,client2,client3);
        await _context.SaveChangesAsync();
        var query = new GetCustomerQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: ""
        );

        // Act
        var result = await _getCustomerQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
    }
}
