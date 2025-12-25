//public class CreateProviderCommandHandlerTest
//{
//    private readonly SalesContext _context;
//    private readonly Mock<ILogger<CreateProviderCommandHandler>> _loggerMock;
//    private readonly CreateProviderCommandHandler _handler;
//    public CreateProviderCommandHandlerTest()
//    {
//        var options = new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(databaseName: "CreateProviderTestsDb")
//            .Options;
//        _context = new SalesContext(options);
//        _loggerMock = new Mock<ILogger<CreateProviderCommandHandler>>();
//        _handler = new CreateProviderCommandHandler(_context, _loggerMock.Object);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnFailResult_WhenProviderNameExists()
//    {
//        // Arrange: un fournisseur avec le même nom existe déjà
//        var existingProvider = Fournisseur.CreateProvider(
//            nom: "ProviderExists",
//            tel: "123456789",
//            fax: null,
//            matricule: null,
//            code: null,
//            codeCat: null,
//            etbSec: null,
//            mail: null,
//            mailDeux: null,
//            constructeur: false,
//            adresse: null
//        );
//        _ = _context.Fournisseur.Add(existingProvider);
//        _ = await _context.SaveChangesAsync();
//        var command = new CreateProviderCommand(
//            Nom: "ProviderExists",
//            Tel: "987654321",
//            Fax: null,
//            Matricule: null,
//            Code: null,
//            CodeCat: null,
//            EtbSec: null,
//            Mail: null,
//            MailDeux: null,
//            Constructeur: false,
//            Adresse: null
//        );
//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);
//        // Assert
//        Assert.True(result.IsFailed);
//        Assert.Contains("provider_name_exists", result.Errors[0].Message);
//    }

//    [Fact]
//    public async Task Handle_ShouldCreateAndReturnId_WhenProviderNameIsUnique()
//    {
//        // Arrange: commande avec un nom unique
//        var command = new CreateProviderCommand(
//            Nom: "NewProvider",
//            Tel: "123456789",
//            Fax: "Fax",
//            Matricule: "Mat123",
//            Code: "CodeX",
//            CodeCat: "Cat1",
//            EtbSec: "EtbSec",
//            Mail: "mail@example.com",
//            MailDeux: "mail2@example.com",
//            Constructeur: true,
//            Adresse: "123 Main St"
//        );
//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);
//        // Assert
//        Assert.True(result.IsSuccess);
//        Assert.True(result.Value > 0); 
//        var providerInDb = await _context.Fournisseur.FindAsync(result.Value);
//        Assert.NotNull(providerInDb);
//        Assert.Equal("NewProvider", providerInDb.Nom);
//        Assert.Equal("123456789", providerInDb.Tel);
//    }
//}
