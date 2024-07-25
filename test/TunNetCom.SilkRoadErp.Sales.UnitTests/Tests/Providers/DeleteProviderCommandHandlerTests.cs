namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers;
public class DeleteProviderCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<DeleteProviderCommandHandler> _testLogger;
    private readonly DeleteProviderCommandHandler _handler;

    public DeleteProviderCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
            _context = new SalesContext(options);
            _testLogger = new TestLogger<DeleteProviderCommandHandler>();
            _handler = new DeleteProviderCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ProviderNotFound_ReturnError()
    {
        //Arrange
        var command = new DeleteProviderCommand(id: 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("provider_not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fournisseur with ID: {command} not found"));
    }

    [Fact]
    public async Task Handle_ProviderDeleted_ReturnSuccess()
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
        var command = new DeleteProviderCommand(id: provider.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_context.Fournisseur, p => p.Id == provider.Id);
    }

    [Fact]
    public async Task Handle_LogsProviderDeleted()
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
        var command = new DeleteProviderCommand(id: provider.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Delete Fournisseur with values: {command}"));
    }

}