using System.Reflection.Metadata;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers;

public class UpdateCustomerCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<UpdateCustomerCommandHandler> _testLogger;
    private readonly UpdateCustomerCommandHandler _updateCustomerCommandHandler;

    public UpdateCustomerCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
        _context = new SalesContext(options);
        _testLogger = new TestLogger<UpdateCustomerCommandHandler>();
        _updateCustomerCommandHandler = new UpdateCustomerCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailResult()
    {
        // Arrange
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "Updated Customer",
            Tel: "1234567898",
            Adresse: "Updated Address",
            Matricule: "Updated Matricule",
            Code: "Updated Code",
            CodeCat: "Updated CodeCat",
            EtbSec: "Updated EtbSec",
            Mail: "updatedemail@example.com");

        // Act
        var result = await _updateCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains("Customer not found with ID: 1"));
    }

    [Fact]
    public async Task Handle_CustomerNameExists_ReturnsFailResult()
    {
        // Arrange
        var existingCustomer = Client.CreateClient(
            nom: "Existing Customer",
            tel: "1234567898",
            adresse: "Address",
            matricule: "Matricule",
            code: "Code",
            codeCat: "CodeCat",
            etbSec: "EtbSec",
            mail: "email@example.com");

        _context.Client.Add(existingCustomer);
        await _context.SaveChangesAsync();

        var command = new UpdateCustomerCommand(
            Id: existingCustomer.Id,
            Nom: "Existing Customer",
            Tel: "1234567898",
            Adresse: "Updated Address",
            Matricule: "Updated Matricule",
            Code: "Updated Code",
            CodeCat: "Updated CodeCat",
            EtbSec: "Updated EtbSec",
            Mail: "updatedemail@example.com");

        // Act
        var result = await _updateCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("customer_name_exist", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccessResult()
    {
        // Arrange
        var existingCustomer = Client.CreateClient(
            nom: "Existing Customer",
            tel: "1234567898",
            adresse: "Address",
            matricule: "Matricule",
            code: "Code",
            codeCat: "CodeCat",
            etbSec: "EtbSec",
            mail: "email@example.com");

        _context.Client.Add(existingCustomer);
        await _context.SaveChangesAsync();

        var command = new UpdateCustomerCommand(
            Id: existingCustomer.Id,
            Nom: "Updated Customer",
            Tel: "1234567899",
            Adresse: "Updated Address",
            Matricule: "Updated Matricule",
            Code: "Updated Code",
            CodeCat: "Updated CodeCat",
            EtbSec: "Updated EtbSec",
            Mail: "updatedemail@example.com");

        // Act
        var result = await _updateCustomerCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Customer updated with ID: {existingCustomer.Id} updated successfully"));
    }
}
