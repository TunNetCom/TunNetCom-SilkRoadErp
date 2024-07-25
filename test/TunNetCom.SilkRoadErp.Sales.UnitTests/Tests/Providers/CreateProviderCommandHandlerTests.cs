namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers;
    public class CreateProviderCommandHandlerTests
    {
    private readonly SalesContext _context;
    private readonly TestLogger<CreateProviderCommandHandler> _testLogger;
    private readonly CreateProviderCommandHandler _handler;

    public CreateProviderCommandHandlerTests() {

        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
            _context = new SalesContext(options);
            _testLogger = new TestLogger<CreateProviderCommandHandler>();
            _handler = new CreateProviderCommandHandler(_context, _testLogger);
    }

    [Fact]
    public async Task Handle_ProviderNameExists_ReturnsFailResult()
    {
        // Arrange
        var command = new CreateProviderCommand(
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
            Adresse: "adresse"
            );

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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("provider_name_exists", result.Errors.First().Message);

    }

    [Fact]
    public async Task Handle_CreateProvider_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateProviderCommand(
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
            Adresse: "adresse"
            );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_LogsCustomerCreated()
    {
        // Arrange
        var command = new CreateProviderCommand(
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
            Adresse: "adresse"
            );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Creating Fournisseur with values: {command}"));
    }

    [Fact] 
    public async Task Handle_LogsCustomerCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateProviderCommand(
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
            Adresse: "adresse"
            );

        var provider = Fournisseur.CreateProvider(
            nom: command.Nom,
            tel: command.Tel,
            fax: command.Fax,
            matricule: command.Matricule,
            code: command.Code,
            codeCat: command.CodeCat,
            etbSec: command.EtbSec,
            mail: command.Mail,
            mailDeux: command.MailDeux,
            constructeur: command.Constructeur,
            adresse: command.Adresse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Expected operation to succeed");
        Assert.Contains(_testLogger.Logs, log => log.Contains($"Fournisseur created successfully with ID: {result.Value}"));
    }
}


