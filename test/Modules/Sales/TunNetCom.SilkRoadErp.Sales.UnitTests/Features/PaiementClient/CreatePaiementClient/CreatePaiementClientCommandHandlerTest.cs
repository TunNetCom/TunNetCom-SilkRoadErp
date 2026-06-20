using TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.CreatePaiementClient;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.PaiementClient.CreatePaiementClient;

public class CreatePaiementClientCommandHandlerTest
{
    private static SalesContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SalesContext(options);
    }

    private static CreatePaiementClientCommand BuildValidCommand(int clientId) => new(
        NumeroTransactionBancaire: null,
        ClientId: clientId,
        AccountingYearId: null,
        Montant: 100m,
        DatePaiement: new DateTime(2024, 1, 15),
        MethodePaiement: "Espece",
        FactureIds: null,
        BonDeLivraisonIds: null,
        NumeroChequeTraite: null,
        BanqueId: null,
        DateEcheance: null,
        Commentaire: null,
        DocumentBase64: null);

    private static async Task<(int clientId, int yearId)> SeedClientAndActiveYearAsync(SalesContext context)
    {
        var client = Client.CreateClient(
            "TestClient", "12345678", "Address", "Mat", "Code", "CodeCat", "EtbSec", "mail@example.com");
        _ = context.Client.Add(client);
        var year = AccountingYear.CreateAccountingYear(2024, isActive: true);
        _ = context.AccountingYear.Add(year);
        _ = await context.SaveChangesAsync();
        return (client.Id, year.Id);
    }


    [Fact]
    public async Task Handle_ClientNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId: 999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "client_not_found");
    }

    [Fact]
    public async Task Handle_NoActiveAccountingYear_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var client = Client.CreateClient(
            "TestClient", "12345678", "Address", "Mat", "Code", "CodeCat", "EtbSec", "mail@example.com");
        _ = context.Client.Add(client);
        _ = await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(client.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "no_active_accounting_year");
    }

    [Fact]
    public async Task Handle_AccountingYearIdProvidedButNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { AccountingYearId = 777 };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "no_active_accounting_year");
    }

    [Fact]
    public async Task Handle_NumeroTransactionBancaireAlreadyExists_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, yearId) = await SeedClientAndActiveYearAsync(context);
        _ = context.PaiementClient.Add(TunNetCom.SilkRoadErp.Sales.Domain.Entites.PaiementClient.CreatePaiementClient(
            numeroTransactionBancaire: "DUPLICATE_NUM",
            clientId,
            yearId,
            montant: 50m,
            new DateTime(2024, 1, 1),
            MethodePaiement.Espece,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            documentStoragePath: null));
        _ = await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { NumeroTransactionBancaire = "DUPLICATE_NUM" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "numero_already_exists");
    }

    [Fact]
    public async Task Handle_InvalidMethodePaiement_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { MethodePaiement = "Cash" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "invalid_methode_paiement");
    }

    [Fact]
    public async Task Handle_BothFacturesAndBonDeLivraisons_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with
        {
            FactureIds = new List<int> { 1 },
            BonDeLivraisonIds = new List<int> { 1 }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "cannot_have_both_factures_and_bon_de_livraisons");
    }

    [Fact]
    public async Task Handle_FacturesNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, yearId) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { FactureIds = new List<int> { 404 } };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "factures_not_found: 404");
    }

    [Fact]
    public async Task Handle_BonDeLivraisonsNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, yearId) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { BonDeLivraisonIds = new List<int> { 404 } };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "bon_de_livraisons_not_found: 404");
    }

    [Fact]
    public async Task Handle_BonDeLivraisonAlreadyInvoiced_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, yearId) = await SeedClientAndActiveYearAsync(context);
        var bl = BonDeLivraison.CreateBonDeLivraison(
            date: new DateTime(2024, 1, 5),
            totHTva: 10m,
            totTva: 2m,
            netPayer: 12m,
            tempBl: new TimeOnly(10, 30),
            numFacture: 55, // already invoiced
            clientId: clientId,
            accountingYearId: yearId);
        _ = context.BonDeLivraison.Add(bl);
        _ = await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { BonDeLivraisonIds = new List<int> { bl.Id } };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == $"bon_de_livraison_already_invoiced_attach_payment_to_invoice: {bl.Id}");
    }

    [Fact]
    public async Task Handle_BanqueNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { BanqueId = 888 };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "banque_not_found");
    }

    [Fact]
    public async Task Handle_InvalidDocumentFormat_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();

        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { DocumentBase64 = "not-base64!!" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "invalid_document_format");
        storage.Verify(s => s.SaveAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ErrorStoringDocument_ReturnsFailure()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        _ = storage.Setup(s => s.SaveAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new InvalidOperationException("disk full"));
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { DocumentBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 }) };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().Contain(e => e.Message == "error_storing_document");
    }


    [Fact]
    public async Task Handle_CreatesMinimalPayment_ReturnsNewId()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().BeGreaterThan(0);
        var created = await context.PaiementClient.FirstOrDefaultAsync(p => p.Id == result.Value);
        _ = created.Should().NotBeNull();
        _ = created!.Montant.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_CreatesWithFactureLinks_CreatesJunctionRows()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, yearId) = await SeedClientAndActiveYearAsync(context);
        var facture = new Facture
        {
            Id = 7,
            IdClient = clientId,
            AccountingYearId = yearId,
            Date = new DateTime(2024, 1, 10)
        };
        _ = context.Facture.Add(facture);
        _ = await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { FactureIds = new List<int> { 7 } };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().BeGreaterThan(0);
        var junctions = await context.Set<PaiementClientFacture>()
            .Where(j => j.PaiementClientId == result.Value)
            .ToListAsync();
        _ = junctions.Should().HaveCount(1);
        _ = junctions[0].FactureId.Should().Be(7);
    }

    [Fact]
    public async Task Handle_CreatesWithBonDeLivraisonLinks_CreatesJunctionRows()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, yearId) = await SeedClientAndActiveYearAsync(context);
        var bl = BonDeLivraison.CreateBonDeLivraison(
            date: new DateTime(2024, 1, 5),
            totHTva: 10m,
            totTva: 2m,
            netPayer: 12m,
            tempBl: new TimeOnly(10, 30),
            numFacture: null, // not invoiced -> allowed
            clientId: clientId,
            accountingYearId: yearId);
        _ = context.BonDeLivraison.Add(bl);
        _ = await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with { BonDeLivraisonIds = new List<int> { bl.Id } };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().BeGreaterThan(0);
        var junctions = await context.Set<PaiementClientBonDeLivraison>()
            .Where(j => j.PaiementClientId == result.Value)
            .ToListAsync();
        _ = junctions.Should().HaveCount(1);
        _ = junctions[0].BonDeLivraisonId.Should().Be(bl.Id);
    }

    [Fact]
    public async Task Handle_CreatesWithDocument_StoresPathAndSucceeds()
    {
        // Arrange
        using var context = CreateTestContext();
        var (clientId, _) = await SeedClientAndActiveYearAsync(context);

        var logger = Mock.Of<ILogger<CreatePaiementClientCommandHandler>>();
        var storage = new Mock<IDocumentStorageService>();
        _ = storage.Setup(s => s.SaveAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync("stored/path/paiement.bin");
        var handler = new CreatePaiementClientCommandHandler(context, logger, storage.Object);
        var command = BuildValidCommand(clientId) with
        {
            NumeroTransactionBancaire = "TX001",
            DocumentBase64 = Convert.ToBase64String(new byte[] { 9, 9, 9 })
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().BeGreaterThan(0);
        storage.Verify(
            s => s.SaveAsync(It.IsAny<byte[]>(), It.Is<string>(n => n != null && n.Contains("paiement_client_TX001")), It.IsAny<CancellationToken>()),
            Times.Once);
        var created = await context.PaiementClient.FirstOrDefaultAsync(p => p.Id == result.Value);
        _ = created.Should().NotBeNull();
        _ = created!.DocumentStoragePath.Should().Be("stored/path/paiement.bin");
    }
}
