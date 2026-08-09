using TunNetCom.SilkRoadErp.Sales.Api.Exceptions;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.GetInvoicesByCustomerWithSummary;

public class GetInvoicesByCustomerWithSummaryQueryHandlerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IAccountingYearFinancialParametersService> _financialParamsServiceMock;
    private readonly Mock<ILogger<GetInvoicesByCustomerWithSummaryQueryHandler>> _loggerMock;

    public GetInvoicesByCustomerWithSummaryQueryHandlerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _financialParamsServiceMock = new Mock<IAccountingYearFinancialParametersService>();
        _loggerMock = new Mock<ILogger<GetInvoicesByCustomerWithSummaryQueryHandler>>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"InvoicesByCustomer_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private GetInvoicesByCustomerWithSummaryQueryHandler CreateHandler(SalesContext context)
    {
        return new GetInvoicesByCustomerWithSummaryQueryHandler(
            context,
            _loggerMock.Object,
            _mediatorMock.Object,
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

    private static Facture CreateFacture(int id, int num, DateTime date, int clientId, int yearId, decimal gross = 100, decimal vat = 19, decimal net = 119)
    {
        return new Facture
        {
            Id = id,
            Num = num,
            Date = date,
            IdClient = clientId,
            AccountingYearId = yearId,
            BonDeLivraison = new List<BonDeLivraison>
            {
                new()
                {
                    Num = num,
                    Date = date,
                    TotHTva = gross,
                    TotTva = vat,
                    NetPayer = net,
                    TempBl = new TimeOnly(10, 0),
                    ClientId = clientId,
                    AccountingYearId = yearId
                }
            }
        };
    }

    private static (SalesContext Context, int YearId) Setup(SalesContext context)
    {
        var year = AccountingYear.CreateAccountingYear(2024, true);
        _ = context.AccountingYear.Add(year);
        _ = context.SaveChanges();
        SalesContext.SetActiveAccountingYearId(year.Id);
        return (context, year.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenCustomerHasNoInvoices()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 1), 2, yearId));
        _ = context.SaveChanges();
        SetupFinancialParams();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Invoices.Items.Should().BeEmpty();
        _ = result.Value.TotalExcludingTaxAmount.Should().Be(0);
        _ = result.Value.TotalIncludingTaxAmount.Should().Be(0);
        _ = result.Value.TotalVATAmount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldProjectInvoiceWithTotals()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 100, new DateTime(2024, 6, 1), 1, yearId, gross: 200, vat: 26, net: 226));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 2m);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null),
            CancellationToken.None);

        var item = result.Value.Invoices.Items.Should().ContainSingle().Subject;
        _ = item.Id.Should().Be(1);
        _ = item.Number.Should().Be(100);
        _ = item.TotalExcludingTaxAmount.Should().Be(200);
        _ = item.TotalVATAmount.Should().Be(26);
        _ = item.TotalIncludingTaxAmount.Should().Be(228m); // 226 + 2 timbre
        _ = result.Value.TotalExcludingTaxAmount.Should().Be(200);
        _ = result.Value.TotalVATAmount.Should().Be(26);
        _ = result.Value.TotalIncludingTaxAmount.Should().Be(226);
    }

    [Fact]
    public async Task Handle_ShouldSetStatutFromEntity()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        var invoice = CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, yearId);
        invoice.Valider();
        _ = context.Facture.Add(invoice);
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null),
            CancellationToken.None);

        var item = result.Value.Invoices.Items.Should().ContainSingle().Subject;
        _ = item.Statut.Should().Be((int)DocumentStatus.Valid);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatut()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        var invoice1 = CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, yearId);
        invoice1.Valider();
        var invoice2 = CreateFacture(2, 2, new DateTime(2024, 6, 2), 1, yearId);
        _ = context.Facture.Add(invoice1);
        _ = context.Facture.Add(invoice2);
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var valid = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null, Statut: DocumentStatus.Valid),
            CancellationToken.None);
        _ = valid.Value.Invoices.Items.Should().HaveCount(1);
        _ = valid.Value.Invoices.Items[0].Statut.Should().Be((int)DocumentStatus.Valid);

        var draft = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null, Statut: DocumentStatus.Draft),
            CancellationToken.None);
        _ = draft.Value.Invoices.Items.Should().HaveCount(1);
        _ = draft.Value.Invoices.Items[0].Statut.Should().Be((int)DocumentStatus.Draft);
    }

    [Fact]
    public async Task Handle_ShouldSetHasRetenueSource()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 100, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.RetenueSourceClient.Add(new RetenueSourceClient
        {
            NumFacture = 100,
            TauxRetenu = 1,
            MontantApresRetenu = 99m,
            DateCreation = new DateTime(2024, 6, 1),
            AccountingYearId = yearId
        });
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null),
            CancellationToken.None);

        _ = result.Value.Invoices.Items.Should().ContainSingle().Subject
            .HasRetenueSource.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldOnlyReturnDataForActiveAccountingYear()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        var year2025 = AccountingYear.CreateAccountingYear(2025, false);
        _ = context.AccountingYear.Add(year2025);
        _ = context.SaveChanges();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2025, 1, 1), 1, year2025.Id));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, null, null),
            CancellationToken.None);

        _ = result.Value.Invoices.Items.Should().HaveCount(1);
        _ = result.Value.Invoices.Items[0].Number.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldPaginateCorrectly()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        for (int i = 1; i <= 5; i++)
        {
            _ = context.Facture.Add(CreateFacture(i, i, new DateTime(2024, 6, i), 1, yearId));
        }
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var page1 = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 2, null, null),
            CancellationToken.None);

        _ = page1.Value.Invoices.Items.Should().HaveCount(2);
        _ = page1.Value.Invoices.TotalCount.Should().Be(5);
        _ = page1.Value.Invoices.TotalPages.Should().Be(3);
        _ = page1.Value.Invoices.Items[0].Number.Should().Be(1);
        _ = page1.Value.Invoices.Items[1].Number.Should().Be(2);

        var page3 = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 3, 2, null, null),
            CancellationToken.None);

        _ = page3.Value.Invoices.Items.Should().HaveCount(1);
        _ = page3.Value.Invoices.Items[0].Number.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPageNumberInvalid()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        SetupFinancialParams();
        var handler = CreateHandler(context);

        _ = await handler.Invoking(h => h.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 0, 10, null, null),
            CancellationToken.None))
            .Should().ThrowAsync<InvalidPaginationParamsException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPageSizeInvalid()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        SetupFinancialParams();
        var handler = CreateHandler(context);

        _ = await handler.Invoking(h => h.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 0, null, null),
            CancellationToken.None))
            .Should().ThrowAsync<InvalidPaginationParamsException>();
    }

    [Fact]
    public async Task Handle_ShouldSortByNumber()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 30, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.Facture.Add(CreateFacture(2, 10, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.Facture.Add(CreateFacture(3, 20, new DateTime(2024, 6, 3), 1, yearId));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "Number", SortConstants.Ascending),
            CancellationToken.None);
        _ = asc.Value.Invoices.Items.Select(i => i.Number).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "Number", SortConstants.Descending),
            CancellationToken.None);
        _ = desc.Value.Invoices.Items.Select(i => i.Number).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByDate()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 3), 1, yearId));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.Facture.Add(CreateFacture(3, 3, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "Date", SortConstants.Ascending),
            CancellationToken.None);
        _ = asc.Value.Invoices.Items.Select(i => i.Date.Date).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "Date", SortConstants.Descending),
            CancellationToken.None);
        _ = desc.Value.Invoices.Items.Select(i => i.Date.Date).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByNetAmount()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, yearId, gross: 100, vat: 19, net: 200));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 2), 1, yearId, gross: 100, vat: 19, net: 100));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "TotalIncludingTaxAmount", SortConstants.Ascending),
            CancellationToken.None);
        _ = asc.Value.Invoices.Items.Select(i => i.TotalIncludingTaxAmount).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "TotalIncludingTaxAmount", SortConstants.Descending),
            CancellationToken.None);
        _ = desc.Value.Invoices.Items.Select(i => i.TotalIncludingTaxAmount).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByGrossAmount()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, yearId, gross: 200));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 2), 1, yearId, gross: 100));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "TotalExcludingTaxAmount", SortConstants.Ascending),
            CancellationToken.None);
        _ = asc.Value.Invoices.Items.Select(i => i.TotalExcludingTaxAmount).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "TotalExcludingTaxAmount", SortConstants.Descending),
            CancellationToken.None);
        _ = desc.Value.Invoices.Items.Select(i => i.TotalExcludingTaxAmount).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldNotThrow_WhenSortPropertyUnknown()
    {
        using var context = CreateContext();
        var (_, yearId) = Setup(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 2, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.Facture.Add(CreateFacture(2, 1, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();
        SetupFinancialParams(timbre: 0);
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesByCustomerWithSummaryQuery(1, 1, 10, "UnknownField", SortConstants.Ascending),
            CancellationToken.None);

        _ = result.Value.Invoices.Items.Should().HaveCount(2);
    }
}
