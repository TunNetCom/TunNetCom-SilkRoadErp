using TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetClientsAvecProblemesSolde;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Soldes.GetClientsAvecProblemesSolde;

public class GetClientsAvecProblemesSoldeQueryHandlerTest
{
    private readonly Mock<ILogger<GetClientsAvecProblemesSoldeQueryHandler>> _loggerMock;
    private readonly Mock<IActiveAccountingYearService> _activeYearServiceMock;
    private readonly Mock<ISoldeClientCalculationService> _soldeServiceMock;

    public GetClientsAvecProblemesSoldeQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetClientsAvecProblemesSoldeQueryHandler>>();
        _activeYearServiceMock = new Mock<IActiveAccountingYearService>();
        _soldeServiceMock = new Mock<ISoldeClientCalculationService>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"ClientsProblemesSolde_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private GetClientsAvecProblemesSoldeQueryHandler CreateHandler(SalesContext context)
    {
        return new GetClientsAvecProblemesSoldeQueryHandler(
            context,
            _loggerMock.Object,
            _activeYearServiceMock.Object,
            _soldeServiceMock.Object);
    }

    private static BonDeLivraison CreateDeliveryNote(
        int num, DateTime date, int clientId, int yearId, int qteLi = 10, int? qteLivree = null)
    {
        return new BonDeLivraison
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
                    RefProduit = $"PRD{num}",
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
    }

    [Fact]
    public async Task Handle_WhenNoActiveYear_ReturnsEmptyPagedList()
    {
        using var context = CreateContext();
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetClientsAvecProblemesSoldeQuery(1, 10),
            CancellationToken.None);

        _ = result.Items.Should().BeEmpty();
        _ = result.TotalCount.Should().Be(0);
        _ = result.CurrentPage.Should().Be(1);
        _ = result.PageSize.Should().Be(10);
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
            new GetClientsAvecProblemesSoldeQuery(1, 10, AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.Items.Should().BeEmpty();
        _activeYearServiceMock.Verify(
            s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoClientHasProblems_ReturnsEmpty()
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
            new GetClientsAvecProblemesSoldeQuery(1, 10, AccountingYearId: 2024),
            CancellationToken.None);

        _ = result.Items.Should().BeEmpty();
        _ = result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenClientsHaveProblems_ReturnsPagedItems()
    {
        using var context = CreateContext();
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(100, new DateTime(2024, 6, 1), 1, 2024, qteLi: 10, qteLivree: 4));
        _ = context.SaveChanges();
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoldeClientItemDto>
            {
                new()
                {
                    ClientId = 1, ClientNom = "Alpha", Solde = -500m,
                    TotalFactures = 1000m, TotalPaiements = 500m,
                    DateDernierDocument = new DateTime(2024, 6, 1)
                },
                new()
                {
                    ClientId = 2, ClientNom = "Beta", Solde = 0m,
                    TotalFactures = 0m, TotalPaiements = 0m,
                    DateDernierDocument = null
                }
            });
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetClientsAvecProblemesSoldeQuery(1, 10, AccountingYearId: 2024),
            CancellationToken.None);

        var item = result.Items.Should().ContainSingle().Subject;
        _ = item.ClientId.Should().Be(1);
        _ = item.ClientNom.Should().Be("Alpha");
        _ = item.Solde.Should().Be(-500m);
        _ = item.TotalFactures.Should().Be(1000m);
        _ = item.TotalPaiements.Should().Be(500m);
        _ = item.NombreQuantitesNonLivrees.Should().Be(6); // 10 - 4
        _ = result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenClientHasUndeliveredQuantities_IncludesClient()
    {
        using var context = CreateContext();
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
            new GetClientsAvecProblemesSoldeQuery(1, 10, AccountingYearId: 2024),
            CancellationToken.None);

        var item = result.Items.Should().ContainSingle().Subject;
        _ = item.NombreQuantitesNonLivrees.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ShouldPaginateAndPrioritizeNegativeSoldes()
    {
        using var context = CreateContext();
        var soldes = new List<SoldeClientItemDto>();
        for (int i = 1; i <= 5; i++)
        {
            soldes.Add(new SoldeClientItemDto
            {
                ClientId = i,
                ClientNom = $"Client{i}",
                Solde = i == 5 ? -100m : 10m,
                TotalFactures = 100m,
                TotalPaiements = i == 5 ? 200m : 90m,
                DateDernierDocument = new DateTime(2024, 6, i)
            });
        }
        _ = _soldeServiceMock
            .Setup(s => s.GetSoldesClientsForAccountingYearAsync(2024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(soldes);
        var handler = CreateHandler(context);

        var page1 = await handler.Handle(
            new GetClientsAvecProblemesSoldeQuery(1, 2, AccountingYearId: 2024),
            CancellationToken.None);

        _ = page1.TotalCount.Should().Be(5);
        _ = page1.Items.Should().HaveCount(2);
        _ = page1.Items[0].ClientId.Should().Be(5); // negative solde sorted first

        var page3 = await handler.Handle(
            new GetClientsAvecProblemesSoldeQuery(3, 2, AccountingYearId: 2024),
            CancellationToken.None);

        _ = page3.Items.Should().HaveCount(1);
    }
}
