using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers;

public class CreateCustomerCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<CreateCustomerCommandHandler> _testLogger;
    private readonly CreateCustomerCommandHandler _createCustomerCommandHandler;

    public CreateCustomerCommandHandlerTests()
    {
        //UseInMemoryDatabase for isolated, fast testing without hitting a real database.
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
        .Options;

        _context = new SalesContext(options);
        //Creates a TestLogger instance to mock logging behavior.
        _testLogger = new TestLogger<CreateCustomerCommandHandler>();
        _createCustomerCommandHandler = new CreateCustomerCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_CustomerNameExists_ReturnsFailResult()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            Nom: "Existing Customer",
            Tel: "123456",
            Adresse: "Address",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "CodeCat",
            EtbSec: "EtbSec",
            Mail: "email@example.com");

        var clientDuplicated = Client.CreateClient(
            nom: "Existing Customer",
            tel: "123456",
            adresse: "Address",
            matricule: "Matricule",
            code: "Code",
            codeCat: "CodeCat",
            etbSec: "EtbSec",
            mail: "email@example.com");

        _context.Client.Add(clientDuplicated);
        await _context.SaveChangesAsync();

        // Act
        var result = await _createCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("customer_name_exist", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_NewCustomer_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            Nom: "New Customer",
            Tel: "123456",
            Adresse: "Address",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "CodeCat",
            EtbSec: "EtbSec",
            Mail: "email@example.com");

        // Act
        var result = await _createCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_LogsCustomerCreated()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            Nom: "New Customer",
            Tel: "123456",
            Adresse: "Address",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "CodeCat",
            EtbSec: "EtbSec",
            Mail: "email@example.com");

        // Act
        await _createCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Creating customer with values: {command}"));
    }

    [Fact]
    public async Task Handle_LogsCustomerCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            Nom: "New Customer",
            Tel: "123456",
            Adresse: "Address",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "CodeCat",
            EtbSec: "EtbSec",
            Mail: "email@example.com");

        var client = Client.CreateClient(
            nom: command.Nom,
            tel: command.Tel,
            adresse: command.Adresse,
            matricule: command.Matricule,
            code: command.Code,
            codeCat: command.CodeCat,
            etbSec: command.EtbSec,
            mail: command.Mail);


        // Act
        var result = await _createCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Expected operation to succeed");
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Customer created successfully with ID: {result.Value}"));
    }
}
