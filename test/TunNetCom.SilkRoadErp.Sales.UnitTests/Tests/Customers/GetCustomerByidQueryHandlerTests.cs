using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers;

public class GetCustomerByidQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetCustomerByIdQueryHandler> _testLogger;
    private readonly GetCustomerByIdQueryHandler _getCustomerByIdQueryHandler;

    public GetCustomerByidQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;

        _context = new SalesContext(options);
        _testLogger = new TestLogger<GetCustomerByIdQueryHandler>();
        _getCustomerByIdQueryHandler = new GetCustomerByIdQueryHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsCustomer()
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

        var query = new GetCustomerByIdQuery(client.Id);

        // Act
        var result = await _getCustomerByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(client.Nom, result.Value.Nom);
    }

    [Fact]
    public async Task Handle_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 12; 
        var query = new GetCustomerByIdQuery(invalidId);

        // Act
        var result = await _getCustomerByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("client_not_found", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_LogsClientNotFound()
    {
        // Arrange
        var invalidId = 12; 
        var query = new GetCustomerByIdQuery(invalidId);

        // Act
        var result = await _getCustomerByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"customer with ID: {query.Id} not found"));
    }
}
