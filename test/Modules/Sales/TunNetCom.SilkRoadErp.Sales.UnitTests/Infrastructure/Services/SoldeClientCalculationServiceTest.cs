using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.Services;

public class SoldeClientCalculationServiceTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IAccountingYearFinancialParametersService> _financialParamsServiceMock;
    private readonly Mock<ILogger<SoldeClientCalculationService>> _loggerMock;

    public SoldeClientCalculationServiceTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _financialParamsServiceMock = new Mock<IAccountingYearFinancialParametersService>();
        _loggerMock = new Mock<ILogger<SoldeClientCalculationService>>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"SoldeClientCalculationServiceTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private SoldeClientCalculationService CreateService(SalesContext context)
    {
        return new SoldeClientCalculationService(
            context,
            _mediatorMock.Object,
            _financialParamsServiceMock.Object,
            _loggerMock.Object);
    }

    private void SetupFinancialParams(decimal timbre = 1m)
    {
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAppParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new GetAppParametersResponse { Timbre = timbre }));

        _ = _financialParamsServiceMock
            .Setup(s => s.GetTimbreAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal fallback, CancellationToken _) => fallback);
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

    [Fact]
    public async Task CalculateSoldeClientAsync_WhenClientDoesNotExist_ReturnsNull()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        var service = CreateService(context);

        var result = await service.CalculateSoldeClientAsync(999, 2024, CancellationToken.None);

        _ = result.Should().BeNull();
    }

    [Fact]
    public async Task CalculateSoldeClientAsync_WhenYearDoesNotExist_ReturnsNull()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.CalculateSoldeClientAsync(1, 9999, CancellationToken.None);

        _ = result.Should().BeNull();
    }

    [Fact]
    public async Task CalculateSoldeClientAsync_ComputesTotalsAndSolde()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 1m);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        var year = AccountingYear.CreateAccountingYear(2024, true);
        _ = context.AccountingYear.Add(year);
        _ = context.SaveChanges();

        _ = context.BonDeLivraison.Add(new BonDeLivraison
        {
            Num = 100,
            Date = new DateTime(2024, 6, 1),
            NetPayer = 119m,
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = year.Id,
            NumFacture = 1
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = year.Id
        });
        _ = context.BonDeLivraison.Add(new BonDeLivraison
        {
            Num = 200,
            Date = new DateTime(2024, 6, 2),
            NetPayer = 50m,
            TempBl = new TimeOnly(11, 0),
            ClientId = 1,
            AccountingYearId = year.Id,
            NumFacture = null
        });
        _ = context.PaiementClient.Add(PaiementClient.CreatePaiementClient(
            numeroTransactionBancaire: "TRX-1",
            clientId: 1,
            accountingYearId: year.Id,
            montant: 30m,
            datePaiement: new DateTime(2024, 6, 3),
            methodePaiement: MethodePaiement.Espece,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            documentStoragePath: null));
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.CalculateSoldeClientAsync(1, year.Id, CancellationToken.None);

        _ = result.Should().NotBeNull();
        _ = result!.TotalFactures.Should().Be(120m); // 119 + 1 timbre
        _ = result.TotalBonsLivraisonNonFactures.Should().Be(50m);
        _ = result.TotalPaiements.Should().Be(30m);
        _ = result.TotalAvoirs.Should().Be(0m);
        _ = result.TotalFacturesAvoir.Should().Be(0m);
        _ = result.Solde.Should().Be(30m - 120m - 50m);
    }

    [Fact]
    public async Task GetSoldesClientsForAccountingYearAsync_WhenNoActivity_ReturnsEmpty()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.GetSoldesClientsForAccountingYearAsync(2024, CancellationToken.None);

        _ = result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSoldesClientsForAccountingYearAsync_ComputesSoldePerClient()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 1m);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Client.Add(CreateClient(2, "Beta"));
        var year = AccountingYear.CreateAccountingYear(2024, true);
        _ = context.AccountingYear.Add(year);
        _ = context.SaveChanges();

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = year.Id,
            BonDeLivraison = new List<BonDeLivraison>
            {
                new()
                {
                    Num = 100,
                    Date = new DateTime(2024, 6, 1),
                    NetPayer = 100m,
                    TempBl = new TimeOnly(10, 0),
                    ClientId = 1,
                    AccountingYearId = year.Id,
                    NumFacture = 1
                }
            }
        });
        _ = context.BonDeLivraison.Add(new BonDeLivraison
        {
            Num = 200,
            Date = new DateTime(2024, 6, 2),
            NetPayer = 25m,
            TempBl = new TimeOnly(11, 0),
            ClientId = 2,
            AccountingYearId = year.Id,
            NumFacture = null
        });
        _ = context.SaveChanges();

        var service = CreateService(context);
        var result = await service.GetSoldesClientsForAccountingYearAsync(year.Id, CancellationToken.None);

        _ = result.Should().HaveCount(2);
        var alpha = result.Should().ContainSingle(i => i.ClientId == 1).Subject;
        _ = alpha.TotalFactures.Should().Be(101m); // 100 + 1 timbre
        _ = alpha.Solde.Should().Be(-101m);
        _ = alpha.DateDernierDocument.Should().NotBeNull();

        var beta = result.Should().ContainSingle(i => i.ClientId == 2).Subject;
        _ = beta.TotalBonsLivraisonNonFactures.Should().Be(25m);
        _ = beta.Solde.Should().Be(-25m);
    }
}
