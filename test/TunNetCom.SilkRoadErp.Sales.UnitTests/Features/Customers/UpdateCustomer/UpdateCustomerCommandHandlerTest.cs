using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;
public class UpdateCustomerCommandHandlerTests
{
    private DbContextOptions<SalesContext> _dbOptions;
    public UpdateCustomerCommandHandlerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesTestDb")
            .Options;
    }
    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCustomerIsUpdatedSuccessfully()
    {
        // Arrange
        using var context = new SalesContext(_dbOptions);
        context.Database.EnsureDeleted(); 
        context.Database.EnsureCreated();
        var existingClient = Client.CreateClient("Old Name", null, null, null, null, null, null, null);
        existingClient.SetId(1);
        context.Client.Add(existingClient);
        await context.SaveChangesAsync();
        var loggerMock = new Mock<ILogger<UpdateCustomerCommandHandler>>();
        var handler = new UpdateCustomerCommandHandler(context, loggerMock.Object);
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "New Name",
            Tel: "123456",
            Adresse: "Address 123",
            Matricule: "MAT123",
            Code: "C001",
            CodeCat: "CAT01",
            EtbSec: "ETB01",
            Mail: "client@mail.com"
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        var updated = await context.Client.FindAsync(1);
        updated!.Nom.Should().Be("New Name");
        updated.Tel.Should().Be("123456");
        updated.Adresse.Should().Be("Address 123");
        updated.Matricule.Should().Be("MAT123");
    }
    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCustomerNotFound()
    {
        // Arrange
        using var context = new SalesContext(_dbOptions);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        var loggerMock = new Mock<ILogger<UpdateCustomerCommandHandler>>();
        var handler = new UpdateCustomerCommandHandler(context, loggerMock.Object);
        var command = new UpdateCustomerCommand(
            Id: 99, // ID inexistant
            Nom: "New Name",
            Tel: null,
            Adresse: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be("not_found"); // <-- corrigé ici
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenNameAlreadyExists()
    {
        using var context = new SalesContext(_dbOptions);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        var client1 = Client.CreateClient("Client A", null, null, null, null, null, null, null);
        client1.SetId(1);
        var client2 = Client.CreateClient("Existing Name", null, null, null, null, null, null, null);
        client2.SetId(2);
        context.Client.AddRange(client1, client2);
        await context.SaveChangesAsync();
        var loggerMock = new Mock<ILogger<UpdateCustomerCommandHandler>>();
        var handler = new UpdateCustomerCommandHandler(context, loggerMock.Object);
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "Existing Name", 
            Tel: null,
            Adresse: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("customer_name_exist");
    }
}
