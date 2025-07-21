namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers;
public class UpdateProviderCommandHandlerTest
{
    private readonly SalesContext _context;
    private readonly TestLogger<UpdateProviderCommandHandler> _testLogger;
    private readonly UpdateProviderCommandHandler _handler;
    public UpdateProviderCommandHandlerTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base unique à chaque test
            .Options;
        _context = new SalesContext(options);
        _testLogger = new TestLogger<UpdateProviderCommandHandler>();
        _handler = new UpdateProviderCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProviderNotFound()
    {
        // Arrange
        var command = new UpdateProviderCommand(
            Id: 9999, // ID inexistant
            Nom: "New Name",
            Tel: "123",
            Fax: "Fax",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "Cat",
            EtbSec: "Etb",
            Mail: "mail@example.com",
            MailDeux: "mail2@example.com",
            Constructeur: false,
            Adresse: "Adresse");
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains("Fournisseur with ID: 9999 not found"));
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProviderNameAlreadyExists()
    {
        // Arrange
        var provider1 = Fournisseur.CreateProvider("Dup Name", "1", "fax", "m", "c", "cat", "e", "m1", "m2", true, "adr");
        var provider2 = Fournisseur.CreateProvider("Other", "2", "fax", "m", "c", "cat", "e", "m1", "m2", true, "adr");
        _context.Fournisseur.AddRange(provider1, provider2);
        await _context.SaveChangesAsync();
        var command = new UpdateProviderCommand(
            Id: provider2.Id,
            Nom: "Dup Name", // Même nom que provider1
            Tel: "2",
            Fax: "fax",
            Matricule: "m",
            Code: "c",
            CodeCat: "cat",
            EtbSec: "e",
            Mail: "m1",
            MailDeux: "m2",
            Constructeur: true,
            Adresse: "adr");
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("provider_name_exists", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSuccessfully_WhenValidData()
    {
        // Arrange
        var provider = Fournisseur.CreateProvider("Original", "1", "fax", "m", "c", "cat", "e", "m1", "m2", true, "adr");
        _context.Fournisseur.Add(provider);
        await _context.SaveChangesAsync();
        var command = new UpdateProviderCommand(
            Id: provider.Id,
            Nom: "Updated Name",
            Tel: "999",
            Fax: "Updated Fax",
            Matricule: "Updated Mat",
            Code: "Updated Code",
            CodeCat: "Updated Cat",
            EtbSec: "Updated Etb",
            Mail: "new@example.com",
            MailDeux: "new2@example.com",
            Constructeur: false,
            Adresse: "New Address");
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fournisseur with ID: {provider.Id} updated successfully"));
        var updated = await _context.Fournisseur.FindAsync(provider.Id);
        Assert.Equal("Updated Name", updated.Nom);
        Assert.Equal("new@example.com", updated.Mail);
    }
}
