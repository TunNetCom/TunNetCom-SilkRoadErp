using TunNetCom.SilkRoadErp.Sales.Api.Exceptions;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.GetInvoicesWithSummaries;

public class GetInvoicesWithSummariesQueryHandlerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IAccountingYearFinancialParametersService> _financialParamsServiceMock;
    private readonly Mock<ILogger<GetInvoicesWithSummariesQueryHandler>> _loggerMock;

    public GetInvoicesWithSummariesQueryHandlerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _financialParamsServiceMock = new Mock<IAccountingYearFinancialParametersService>();
        _loggerMock = new Mock<ILogger<GetInvoicesWithSummariesQueryHandler>>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"InvoicesSummaries_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private GetInvoicesWithSummariesQueryHandler CreateHandler(SalesContext context)
    {
        return new GetInvoicesWithSummariesQueryHandler(
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

    private static Facture CreateFacture(int id, int num, DateTime date, int clientId, int yearId, decimal gross = 100, decimal vat = 19)
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
                    NetPayer = gross + vat,
                    TempBl = new TimeOnly(10, 0),
                    ClientId = clientId,
                    AccountingYearId = yearId
                }
            }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoInvoices()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesWithSummariesQuery(
                PageNumber: 1, PageSize: 10, CustomerId: null,
                SortOrder: null, SortProperty: null, SearchKeyword: null,
                StartDate: null, EndDate: null),
            CancellationToken.None);

        _ = result.TotalNetAmount.Should().Be(0);
        _ = result.TotalVatAmount.Should().Be(0);
        _ = result.Invoices.Items.Should().BeEmpty();
        _ = result.Invoices.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldProjectInvoiceWithTotals()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 2m);
        var client = CreateClient(1, "Client Alpha");
        _ = context.Client.Add(client);
        _ = context.Facture.Add(CreateFacture(1, 100, new DateTime(2024, 6, 1), 1, 2024, gross: 200, vat: 26));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, null, null, null, null, null),
            CancellationToken.None);

        var item = result.Invoices.Items.Should().ContainSingle().Subject;
        _ = item.Number.Should().Be(100);
        _ = item.CustomerId.Should().Be(1);
        _ = item.CustomerName.Should().Be("Client Alpha");
        _ = item.NetAmount.Should().Be(202m); // 200 HT + 2 timbre
        _ = item.VatAmount.Should().Be(26m);
        _ = result.TotalNetAmount.Should().Be(202m);
        _ = result.TotalVatAmount.Should().Be(26m);
    }

    [Fact]
    public async Task Handle_ShouldFilterByCustomerId()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Client.Add(CreateClient(2, "Beta"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, 2024));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 2), 2, 2024));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, CustomerId: 1, null, null, null, null, null),
            CancellationToken.None);

        _ = result.Invoices.Items.Should().HaveCount(1);
        _ = result.Invoices.Items[0].CustomerId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStartDateAndEndDate()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 5, 1), 1, 2024));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 15), 1, 2024));
        _ = context.Facture.Add(CreateFacture(3, 3, new DateTime(2024, 7, 1), 1, 2024));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesWithSummariesQuery(
                1, 10, null, null, null, null,
                StartDate: new DateTime(2024, 6, 1),
                EndDate: new DateTime(2024, 6, 30)),
            CancellationToken.None);

        _ = result.Invoices.Items.Should().HaveCount(1);
        _ = result.Invoices.Items[0].Number.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchKeyword_OnCustomerNameOrNumber()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha Corp"));
        _ = context.Client.Add(CreateClient(2, "Beta Inc"));
        _ = context.Facture.Add(CreateFacture(1, 100, new DateTime(2024, 6, 1), 1, 2024));
        _ = context.Facture.Add(CreateFacture(2, 200, new DateTime(2024, 6, 2), 2, 2024));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var byName = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, null, null, "Alpha", null, null),
            CancellationToken.None);
        _ = byName.Invoices.Items.Should().HaveCount(1);
        _ = byName.Invoices.Items[0].CustomerName.Should().Be("Alpha Corp");

        var byNumber = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, null, null, "200", null, null),
            CancellationToken.None);
        _ = byNumber.Invoices.Items.Should().HaveCount(1);
        _ = byNumber.Invoices.Items[0].Number.Should().Be(200);
    }

    [Fact]
    public async Task Handle_ShouldPaginateCorrectly()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        for (int i = 1; i <= 5; i++)
        {
            _ = context.Facture.Add(CreateFacture(i, i, new DateTime(2024, 6, i), 1, 2024));
        }
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var page1 = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 2, null, null, null, null, null, null),
            CancellationToken.None);

        _ = page1.Invoices.Items.Should().HaveCount(2);
        _ = page1.Invoices.TotalCount.Should().Be(5);
        _ = page1.Invoices.TotalPages.Should().Be(3);
        _ = page1.Invoices.Items[0].Number.Should().Be(1);
        _ = page1.Invoices.Items[1].Number.Should().Be(2);

        var page3 = await handler.Handle(
            new GetInvoicesWithSummariesQuery(3, 2, null, null, null, null, null, null),
            CancellationToken.None);

        _ = page3.Invoices.Items.Should().HaveCount(1);
        _ = page3.Invoices.Items[0].Number.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPageNumberInvalid()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        var handler = CreateHandler(context);

        _ = await handler.Invoking(h => h.Handle(
            new GetInvoicesWithSummariesQuery(0, 10, null, null, null, null, null, null),
            CancellationToken.None))
            .Should().ThrowAsync<InvalidPaginationParamsException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPageSizeInvalid()
    {
        using var context = CreateContext();
        SetupFinancialParams();
        var handler = CreateHandler(context);

        _ = await handler.Invoking(h => h.Handle(
            new GetInvoicesWithSummariesQuery(1, 0, null, null, null, null, null, null),
            CancellationToken.None))
            .Should().ThrowAsync<InvalidPaginationParamsException>();
    }

    [Fact]
    public async Task Handle_ShouldSortByNumber()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 30, new DateTime(2024, 6, 1), 1, 2024));
        _ = context.Facture.Add(CreateFacture(2, 10, new DateTime(2024, 6, 2), 1, 2024));
        _ = context.Facture.Add(CreateFacture(3, 20, new DateTime(2024, 6, 3), 1, 2024));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Ascending, "Number", null, null, null),
            CancellationToken.None);
        _ = asc.Invoices.Items.Select(i => i.Number).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Descending, "Number", null, null, null),
            CancellationToken.None);
        _ = desc.Invoices.Items.Select(i => i.Number).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByDate()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 3), 1, 2024));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 1), 1, 2024));
        _ = context.Facture.Add(CreateFacture(3, 3, new DateTime(2024, 6, 2), 1, 2024));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Ascending, "Date", null, null, null),
            CancellationToken.None);
        _ = asc.Invoices.Items.Select(i => i.Date.Date).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Descending, "Date", null, null, null),
            CancellationToken.None);
        _ = desc.Invoices.Items.Select(i => i.Date.Date).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByNetAmount()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 1, new DateTime(2024, 6, 1), 1, 2024, gross: 200));
        _ = context.Facture.Add(CreateFacture(2, 2, new DateTime(2024, 6, 2), 1, 2024, gross: 100));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var asc = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Ascending, "NetAmount", null, null, null),
            CancellationToken.None);
        _ = asc.Invoices.Items.Select(i => i.NetAmount).Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Descending, "NetAmount", null, null, null),
            CancellationToken.None);
        _ = desc.Invoices.Items.Select(i => i.NetAmount).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldNotThrow_WhenSortPropertyUnknown()
    {
        using var context = CreateContext();
        SetupFinancialParams(timbre: 0);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateFacture(1, 2, new DateTime(2024, 6, 1), 1, 2024));
        _ = context.Facture.Add(CreateFacture(2, 1, new DateTime(2024, 6, 2), 1, 2024));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new GetInvoicesWithSummariesQuery(1, 10, null, SortConstants.Ascending, "UnknownField", null, null, null),
            CancellationToken.None);

        _ = result.Invoices.Items.Should().HaveCount(2);
    }
}
