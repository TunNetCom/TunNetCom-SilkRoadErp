namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers;

public class DeleteCustomerCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<DeleteCustomerCommandHandler> _testLogger;
    private readonly DeleteCustomerCommandHandler _deleteCustomerCommandHandler;

    public DeleteCustomerCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;

        _context = new SalesContext(options);
        _testLogger = new TestLogger<DeleteCustomerCommandHandler>();
        _deleteCustomerCommandHandler = new DeleteCustomerCommandHandler(_context, _testLogger);
    }
    [Fact]
    public async Task Handle_CustomerNotFound_ReturnError()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Id: 99);

        // Act
        var result = await _deleteCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Customer_Not_Found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Customer with ID: {command} not found"));

    }

    [Fact]
    public async Task Handle_CustomerDeleted_ReturnSuccess()
    {
        // Arrange
        var client = Client.CreateClient
            (
             nom: "Client Test",
             tel: "1234567898",
             adresse: "Address",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "EtbSec",
             mail: "email@example.com");

        _context.Client.Add(client);
        await _context.SaveChangesAsync();

        var command = new DeleteCustomerCommand(Id: client.Id);

        // Act
        var result = await _deleteCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_context.Client, c => c.Id == client.Id);
    }

    [Fact]
    public async Task Handle_LogsCustomerDeleted()
    {
        // Arrange
        var client = Client.CreateClient
            (
             nom: "Client Test",
             tel: "1234567898",
             adresse: "Address",
             matricule: "Matricule",
             code: "Code",
             codeCat: "CodeCat",
             etbSec: "EtbSec",
             mail: "email@example.com");

        _context.Client.Add(client);
        await _context.SaveChangesAsync();

        var command = new DeleteCustomerCommand(Id: client.Id);

        // Act
        var result = await _deleteCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Delete customer with values: {command}"));
    }
}