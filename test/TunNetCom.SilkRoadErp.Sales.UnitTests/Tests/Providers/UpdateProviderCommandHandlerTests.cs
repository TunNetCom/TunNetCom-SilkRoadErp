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
        var command = new UpdateProviderCommand(Id: 1,
            Nom: "Provider",
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
        Assert.Equal("provider_not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fournisseur with ID: {command} not found"));
    }

    [Fact]
    public async Task Handle_ProviderNameExists_ReturnsFailResult()
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

        var command = new UpdateProviderCommand(Id: provider.Id,
            Nom: "Provider",
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
        Assert.Equal("provider_name_exists", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccessResult()
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

        var command = new UpdateProviderCommand(Id: provider.Id,
            Nom: "Updated Provider",
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
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fournisseur updated with ID: {provider.Id} updated successfully"));
    }
}
