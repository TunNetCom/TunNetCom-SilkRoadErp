using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeClient;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using PaiementClientEntity = TunNetCom.SilkRoadErp.Sales.Domain.Entites.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Soldes.GetSoldeClient;

public class GetSoldeClientQueryHandlerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IActiveAccountingYearService> _activeYearServiceMock;
    private readonly Mock<IAccountingYearFinancialParametersService> _financialParamsServiceMock;
    private readonly Mock<ILogger<GetSoldeClientQueryHandler>> _loggerMock;

    public GetSoldeClientQueryHandlerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _activeYearServiceMock = new Mock<IActiveAccountingYearService>();
        _financialParamsServiceMock = new Mock<IAccountingYearFinancialParametersService>();
        _loggerMock = new Mock<ILogger<GetSoldeClientQueryHandler>>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"GetSoldeClientTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private GetSoldeClientQueryHandler CreateHandler(SalesContext context)
    {
        return new GetSoldeClientQueryHandler(
            context,
            _loggerMock.Object,
            _mediatorMock.Object,
            _activeYearServiceMock.Object,
            _financialParamsServiceMock.Object);
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
    public async Task Handle_WhenClientNotFound_ReturnsFailure()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        var handler = CreateHandler(context);

        var result = await handler.Handle(new GetSoldeClientQuery(999), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "client_not_found");
    }

    [Fact]
    public async Task Handle_WhenNoAccountingYearProvidedAndNoneActive_ReturnsFailure()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.SaveChanges();
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "no_active_accounting_year");
    }

    [Fact]
    public async Task Handle_WhenAccountingYearProvided_DoesNotCallActiveYearService()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1, AccountingYearId: 2024), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _activeYearServiceMock.Verify(
            s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComputesTotalsFromFacturesBlsAndPaiements()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 1m);
        _ = context.Client.Add(CreateClient(1, "Alpha"));

        // Factured BL -> facture total = NetPayer + timbre
        var bl = new BonDeLivraison
        {
            Num = 100,
            Date = new DateTime(2024, 6, 1),
            NetPayer = 119m,
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = 2024,
            NumFacture = 1,
            LigneBl = new List<LigneBl>
            {
                new() { RefProduit = "A", DesignationLi = "A", QteLi = 1, PrixHt = 100, TotHt = 100, Tva = 19, TotTtc = 119 }
            }
        };
        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 2024,
            BonDeLivraison = new List<BonDeLivraison> { bl }
        });

        // Non-factured BL
        _ = context.BonDeLivraison.Add(new BonDeLivraison
        {
            Num = 200,
            Date = new DateTime(2024, 6, 2),
            NetPayer = 50m,
            TempBl = new TimeOnly(11, 0),
            ClientId = 1,
            AccountingYearId = 2024,
            NumFacture = null
        });

        // Payment
        _ = context.PaiementClient.Add(PaiementClientEntity.CreatePaiementClient(
            numeroTransactionBancaire: "TRX-1",
            clientId: 1,
            accountingYearId: 2024,
            montant: 30m,
            datePaiement: new DateTime(2024, 6, 3),
            methodePaiement: MethodePaiement.Cheque,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            documentStoragePath: null));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1, AccountingYearId: 2024), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var response = result.Value;
        _ = response.ClientId.Should().Be(1);
        _ = response.ClientNom.Should().Be("Alpha");
        _ = response.TotalFactures.Should().Be(120m); // 119 + 1 timbre
        _ = response.TotalBonsLivraisonNonFactures.Should().Be(50m);
        _ = response.TotalPaiements.Should().Be(30m);
        _ = response.TotalAvoirs.Should().Be(0m);
        _ = response.TotalFacturesAvoir.Should().Be(0m);
        _ = response.Solde.Should().Be(30m - 120m - 50m);
        _ = response.Documents.Should().HaveCount(2);
        _ = response.Documents.Should().Contain(d => d.Type == DocumentTypes.Facture);
        _ = response.Documents.Should().Contain(d => d.Type == "BonDeLivraison");
    }

    [Fact]
    public async Task Handle_WhenRetenueExists_UsesMontantApresRetenu()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 1m);
        _ = context.Client.Add(CreateClient(1, "Alpha"));

        var bl = new BonDeLivraison
        {
            Num = 100,
            Date = new DateTime(2024, 6, 1),
            NetPayer = 1000m,
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = 2024,
            NumFacture = 1
        };
        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 2024,
            BonDeLivraison = new List<BonDeLivraison> { bl }
        });
        _ = context.RetenueSourceClient.Add(new RetenueSourceClient
        {
            NumFacture = 1,
            MontantAvantRetenu = 1000m,
            TauxRetenu = 1,
            MontantApresRetenu = 990m,
            DateCreation = new DateTime(2024, 6, 1),
            AccountingYearId = 2024
        });
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1, AccountingYearId: 2024), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.TotalFactures.Should().Be(990m); // retenue overrides NetPayer + timbre
    }

    [Fact]
    public async Task Handle_WithAvoirsAndFacturesAvoir_ComputesTotals()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));

        var avoirDirect = Avoirs.CreateAvoir(new DateTime(2024, 6, 1), clientId: 1, accountingYearId: 2024);
        avoirDirect.LigneAvoirs.Add(new LigneAvoirs
        {
            RefProduit = "A", DesignationLi = "A", QteLi = 1, PrixHt = 10, TotHt = 10, Tva = 19, TotTtc = 11.9m
        });
        _ = context.Avoirs.Add(avoirDirect);

        var avoirFactureAvoir = Avoirs.CreateAvoir(new DateTime(2024, 6, 2), clientId: 1, accountingYearId: 2024);
        avoirFactureAvoir.LigneAvoirs.Add(new LigneAvoirs
        {
            RefProduit = "B", DesignationLi = "B", QteLi = 1, PrixHt = 20, TotHt = 20, Tva = 19, TotTtc = 23.8m
        });
        var factureAvoir = FactureAvoirClient.CreateFactureAvoirClient(
            numFactureAvoirClientSurPage: 1, idClient: 1,
            date: new DateTime(2024, 6, 2), numFacture: null, accountingYearId: 2024);
        factureAvoir.Avoirs.Add(avoirFactureAvoir);
        _ = context.FactureAvoirClient.Add(factureAvoir);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1, AccountingYearId: 2024), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var response = result.Value;
        _ = response.TotalAvoirs.Should().Be(11.9m);
        _ = response.TotalFacturesAvoir.Should().Be(23.8m);
        _ = response.Documents.Should().Contain(d => d.Type == "Avoir");
        _ = response.Documents.Should().Contain(d => d.Type == "FactureAvoir");
    }

    [Fact]
    public async Task Handle_WithPaiementAttachedToFacture_BuildsPaiementDocuments()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 1m);
        _ = context.Client.Add(CreateClient(1, "Alpha"));

        var bl = new BonDeLivraison
        {
            Num = 100,
            Date = new DateTime(2024, 6, 1),
            NetPayer = 119m,
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = 2024,
            NumFacture = 1
        };
        var facture = new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 2024,
            BonDeLivraison = new List<BonDeLivraison> { bl }
        };
        _ = context.Facture.Add(facture);
        _ = context.SaveChanges();

        var paiement = PaiementClientEntity.CreatePaiementClient(
            numeroTransactionBancaire: "TRX-1",
            clientId: 1,
            accountingYearId: 2024,
            montant: 120m,
            datePaiement: new DateTime(2024, 6, 5),
            methodePaiement: MethodePaiement.Virement,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            documentStoragePath: null);
        _ = context.PaiementClient.Add(paiement);
        _ = context.SaveChanges();

        paiement.Factures.Add(PaiementClientFacture.Create(paiement.Id, facture.Id));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1, AccountingYearId: 2024), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var payment = result.Value.Paiements.Should().ContainSingle().Subject;
        _ = payment.MethodePaiement.Should().Be("Virement");
        _ = payment.Montant.Should().Be(120m);
        var attached = payment.Factures.Should().ContainSingle().Subject;
        _ = attached.Numero.Should().Be(1);
        _ = attached.MontantTtc.Should().Be(120m); // 119 + 1 timbre
    }

    [Fact]
    public async Task Handle_WhenActiveYearServiceProvidesYear_UsesIt()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 2024
        });
        _ = context.SaveChanges();
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)2024);

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetSoldeClientQuery(1), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.AccountingYearId.Should().Be(2024);
    }
}
