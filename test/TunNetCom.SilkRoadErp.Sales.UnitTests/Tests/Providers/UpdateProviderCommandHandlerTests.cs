namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers;
public class UpdateProviderCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<UpdateProviderCommandHandler> _testLogger;
    private readonly UpdateProviderCommandHandler _handler;

    public UpdateProviderCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
            _context = new SalesContext(options);
            _testLogger = new TestLogger<UpdateProviderCommandHandler>();
            _handler = new UpdateProviderCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ProviderNotFound_ReturnFailResult()
    {
        //Arrange
        var command = new UpdateProviderCommand(
            Id: 25858,
            Nom: "Provider Not Found",
            Tel: "123456789",
            Fax: "Fax",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "CodeCat",
            EtbSec: "etbsec",
            Mail: "email@example.com",
            MailDeux: "email@example.com",
            Constructeur: true,
            Adresse: "adresse");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("not_found", result.Errors.First().Message);
        Assert.Contains(
            _testLogger.Logs,
            log => log.Contains($"{nameof(Fournisseur)} with ID: {command.Id} not found"));
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccessResult()
    {
        //Arrange
        var provider = Fournisseur.CreateProvider(
            nom: "Provider To Update and Valid",
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

        var command = new UpdateProviderCommand(
            Id: provider.Id,
            Nom: "Updated Provider that is Provider To Update and Valid",
            Tel: "123456789",
            Fax: "Fax",
            Matricule: "Matricule",
            Code: "Code",
            CodeCat: "CodeCat",
            EtbSec: "etbsec",
            Mail: "Provideremail@example.com",
            MailDeux: "Provideremail2@example.com",
            Constructeur: true,
            Adresse: "adresse");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(
            _testLogger.Logs,
            log => log.Contains($"{nameof(Fournisseur)} with ID: {provider.Id} updated successfully"));
    }
}
