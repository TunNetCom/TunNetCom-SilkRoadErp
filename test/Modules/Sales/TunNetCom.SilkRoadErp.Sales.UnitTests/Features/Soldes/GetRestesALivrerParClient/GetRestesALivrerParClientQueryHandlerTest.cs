using TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetRestesALivrerParClient;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Soldes.GetRestesALivrerParClient;

public class GetRestesALivrerParClientQueryHandlerTest
{
    private readonly Mock<ILogger<GetRestesALivrerParClientQueryHandler>> _loggerMock;
    private readonly Mock<IActiveAccountingYearService> _activeYearServiceMock;
    private readonly Mock<ISoldeClientCalculationService> _soldeServiceMock;

    public GetRestesALivrerParClientQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetRestesALivrerParClientQueryHandler>>();
        _activeYearServiceMock = new Mock<IActiveAccountingYearService>();
        _soldeServiceMock = new Mock<ISoldeClientCalculationService>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"RestesALivrer_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private GetRestesALivrerParClientQueryHandler CreateHandler(SalesContext context)
    {
        return new GetRestesALivrerParClientQueryHandler(
            context,
            _loggerMock.Object,
            _activeYearServiceMock.Object,
            _soldeServiceMock.Object);
    }

    private static Client CreateClient(int id, string name)
    {
        var client = Client.CreateClient(
            nom: name, tel: "123", adresse: "Tunis",
            matricule: $"M{id}", code: $"C{id}",
            codeCat: "CAT1", etbSec: "ES1", mail: $"{name}@test.com");
        client.SetId(id);
        return client;
    }

    private static BonDeLivraison CreateDeliveryNote(
        int num, DateTime date, int clientId, int yearId, int qteLi = 10, int? qteLivree = null, string? refProduit = null)
    {
        var bl = new BonDeLivraison
        {
            Num = num,
            Date = date,
            TotHTva = 100,
            TotTva = 19,
            NetPayer = 119,
            TempBl = new TimeOnly(10, 0),
            ClientId = clientId,
            AccountingYearId = yearId,
            LigneBl = new List<LigneBl>
            {
                new()
                {
                    RefProduit = refProduit ?? $"PRD{num}",
                    DesignationLi = $"Produit {num}",
                    QteLi = qteLi,
                    PrixHt = 10,
                    TotHt = qteLi * 10,
                    Tva = 19,
                    TotTtc = qteLi * 11.9m,
                    QteLivree = qteLivree
                }
            }
        };
        return bl;
    }

    [Fact]
    public async Task Handle_WhenNoActiveYear_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "no_active_accounting_year");
        _soldeServiceMock.Verify(
            s => s.GetSoldesClientsForAccountingYearAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAccountingYearProvided_DoesNotCallActiveYearService()
    {
        using var context = CreateContext();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>());
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Clients.Should().BeEmpty();
        _activeYearServiceMock.Verify(
            s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoClientHasProblems_ReturnsEmptyList()
    {
        using var context = CreateContext();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>
            {
                new() { ClientId = 1, ClientNom = "Alpha", Solde = 0m }
            });
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Clients.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenClientHasNonZeroSolde_BuildsResponseWithLignes()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(100, new DateTime(2024, 6, 1), 1, 2024, qteLi: 10, qteLivree: 4));
        _ = context.SaveChanges();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>
            {
                new() { ClientId = 1, ClientNom = "Alpha", Solde = -500m, TotalFactures = 1000m, TotalPaiements = 500m }
            });
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var client = result.Value.Clients.Should().ContainSingle().Subject;
        _ = client.ClientId.Should().Be(1);
        _ = client.ClientNom.Should().Be("Alpha");
        _ = client.Solde.Should().Be(-500m);
        var ligne = client.LignesRestesALivrer.Should().ContainSingle().Subject;
        _ = ligne.RefProduit.Should().Be("PRD100");
        _ = ligne.QuantiteRestante.Should().Be(6); // 10 - 4
    }

    [Fact]
    public async Task Handle_WhenClientHasUndeliveredQuantities_IncludesClient()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(100, new DateTime(2024, 6, 1), 1, 2024, qteLi: 5, qteLivree: 0));
        _ = context.SaveChanges();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>
            {
                new() { ClientId = 1, ClientNom = "Alpha", Solde = 0m }
            });
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var client = result.Value.Clients.Should().ContainSingle().Subject;
        _ = client.LignesRestesALivrer.Should().ContainSingle().Subject
            .QuantiteRestante.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WhenFullyDelivered_ExcludesLine()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(100, new DateTime(2024, 6, 1), 1, 2024, qteLi: 10, qteLivree: 10));
        _ = context.SaveChanges();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>
            {
                new() { ClientId = 1, ClientNom = "Alpha", Solde = -10m }
            });
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var client = result.Value.Clients.Should().ContainSingle().Subject;
        _ = client.LignesRestesALivrer.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenMultipleLinesForSameProduct_SumsQuantities()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        var bl1 = CreateDeliveryNote(100, new DateTime(2024, 6, 1), 1, 2024, qteLi: 10, qteLivree: 4, refProduit: "SHARED");
        var bl2 = CreateDeliveryNote(101, new DateTime(2024, 6, 2), 1, 2024, qteLi: 10, qteLivree: 4, refProduit: "SHARED");
        _ = context.BonDeLivraison.Add(bl1);
        _ = context.BonDeLivraison.Add(bl2);
        _ = context.SaveChanges();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>
            {
                new() { ClientId = 1, ClientNom = "Alpha", Solde = -10m }
            });
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetRestesALivrerParClientQuery(AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var ligne = result.Value.Clients.Should().ContainSingle().Subject
            .LignesRestesALivrer.Should().ContainSingle().Subject;
        _ = ligne.RefProduit.Should().Be("SHARED");
        _ = ligne.QuantiteRestante.Should().Be(12); // (10-4) + (10-4)
    }
}
