using TunNetCom.SilkRoadErp.Sales.Api.Exceptions;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;

public class GetDeliveryNotesBaseInfosWithSummariesQueryHandlerTest
{
    private readonly Mock<ILogger<GetDeliveryNotesBaseInfosWithSummariesQueryHandler>> _loggerMock;

    public GetDeliveryNotesBaseInfosWithSummariesQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetDeliveryNotesBaseInfosWithSummariesQueryHandler>>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"DeliveryNoteSummaries_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static (SalesContext Context, int YearId, GetDeliveryNotesBaseInfosWithSummariesQueryHandler Handler) Setup()
    {
        var context = CreateContext();
        var year = AccountingYear.CreateAccountingYear(2024, true);
        _ = context.AccountingYear.Add(year);
        _ = context.SaveChanges();
        SalesContext.SetActiveAccountingYearId(year.Id);
        var loggerMock = new Mock<ILogger<GetDeliveryNotesBaseInfosWithSummariesQueryHandler>>();
        var handler = new GetDeliveryNotesBaseInfosWithSummariesQueryHandler(context, loggerMock.Object);
        return (context, year.Id, handler);
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
        int id, int num, DateTime date, int clientId, int yearId,
        decimal gross = 100, decimal vat = 19, decimal net = 119,
        int? numFacture = null, int? technicianId = null)
    {
        return new BonDeLivraison
        {
            Id = id,
            Num = num,
            Date = date,
            TotHTva = gross,
            TotTva = vat,
            NetPayer = net,
            TempBl = new TimeOnly(10, 0),
            ClientId = clientId,
            AccountingYearId = yearId,
            NumFacture = numFacture,
            InstallationTechnicianId = technicianId
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoDeliveryNotes()
    {
        var (context, _, handler) = Setup();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                PageNumber: 1, PageSize: 10, IsInvoiced: null,
                CustomerId: null, InvoiceId: null, SortOrder: null,
                SortProperty: null, SearchKeyword: null, StartDate: null,
                EndDate: null, Status: null, TechnicianId: null, TagIds: null),
            CancellationToken.None);

        _ = result.TotalGrossAmount.Should().Be(0);
        _ = result.TotalNetAmount.Should().Be(0);
        _ = result.TotalVatAmount.Should().Be(0);
        _ = result.GetDeliveryNoteBaseInfos.Items.Should().BeEmpty();
        _ = result.GetDeliveryNoteBaseInfos.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldProjectDeliveryNoteCorrectly()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client Alpha");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(
            id: 1, num: 100, date: new DateTime(2024, 6, 1),
            clientId: 1, yearId: yearId, gross: 200, vat: 26, net: 226));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        var item = result.GetDeliveryNoteBaseInfos.Items.Should().ContainSingle().Subject;
        _ = item.Id.Should().Be(1);
        _ = item.Number.Should().Be(100);
        _ = item.Date.Date.Should().Be(new DateTime(2024, 6, 1));
        _ = item.GrossAmount.Should().Be(200);
        _ = item.VatAmount.Should().Be(26);
        _ = item.NetAmount.Should().Be(226);
        _ = item.CustomerId.Should().Be(1);
        _ = item.CustomerName.Should().Be("Client Alpha");
        _ = item.NumFacture.Should().BeNull();
        _ = item.Statut.Should().Be((int)DocumentStatus.Draft);
        _ = item.StatutLibelle.Should().Be("Draft");
    }

    [Fact]
    public async Task Handle_ShouldPaginateCorrectly()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        for (int i = 1; i <= 5; i++)
        {
            _ = context.BonDeLivraison.Add(CreateDeliveryNote(
                id: i, num: i, date: new DateTime(2024, 6, i),
                clientId: 1, yearId: yearId));
        }
        _ = context.SaveChanges();

        var page1 = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 2, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = page1.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(2);
        _ = page1.GetDeliveryNoteBaseInfos.TotalCount.Should().Be(5);
        _ = page1.GetDeliveryNoteBaseInfos.CurrentPage.Should().Be(1);
        _ = page1.GetDeliveryNoteBaseInfos.TotalPages.Should().Be(3);
        _ = page1.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(1);
        _ = page1.GetDeliveryNoteBaseInfos.Items[1].Number.Should().Be(2);

        var page2 = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                2, 2, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = page2.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(2);
        _ = page2.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(3);
        _ = page2.GetDeliveryNoteBaseInfos.Items[1].Number.Should().Be(4);

        var page3 = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                3, 2, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = page3.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = page3.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPageNumberInvalid()
    {
        var (context, _, handler) = Setup();

        _ = await handler.Invoking(h => h.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                0, 10, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None))
            .Should().ThrowAsync<InvalidPaginationParamsException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPageSizeInvalid()
    {
        var (context, _, handler) = Setup();

        _ = await handler.Invoking(h => h.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 0, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None))
            .Should().ThrowAsync<InvalidPaginationParamsException>();
    }

    [Fact]
    public async Task Handle_ShouldComputeCorrectTotals()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, gross: 100, vat: 19, net: 119));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId, gross: 200, vat: 26, net: 226));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(3, 3, new DateTime(2024, 6, 3), 1, yearId, gross: 50, vat: 3.5m, net: 53.5m));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.TotalGrossAmount.Should().Be(350);
        _ = result.TotalVatAmount.Should().Be(48.5m);
        _ = result.TotalNetAmount.Should().Be(398.5m);
    }

    [Fact]
    public async Task Handle_ShouldFilterByCustomerId()
    {
        var (context, yearId, handler) = Setup();
        var client1 = CreateClient(1, "Alpha");
        var client2 = CreateClient(2, "Beta");
        _ = context.Client.Add(client1);
        _ = context.Client.Add(client2);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 2, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, 1, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].CustomerId.Should().Be(1);
        _ = result.TotalGrossAmount.Should().Be(100);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStartDate()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 5, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null,
                StartDate: new DateTime(2024, 6, 1), null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterByEndDateInclusive()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 7, 1), 1, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null,
                EndDate: new DateTime(2024, 6, 1), null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);

        var dn1 = CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId);
        var dn2 = CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId);
        dn2.Valider();
        _ = context.BonDeLivraison.Add(dn1);
        _ = context.BonDeLivraison.Add(dn2);
        _ = context.SaveChanges();

        var validResult = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null,
                Status: (int)DocumentStatus.Valid, null, null),
            CancellationToken.None);

        _ = validResult.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = validResult.GetDeliveryNoteBaseInfos.Items[0].Statut.Should().Be((int)DocumentStatus.Valid);

        var draftResult = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null,
                Status: (int)DocumentStatus.Draft, null, null),
            CancellationToken.None);

        _ = draftResult.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = draftResult.GetDeliveryNoteBaseInfos.Items[0].Statut.Should().Be((int)DocumentStatus.Draft);
    }

    [Fact]
    public async Task Handle_ShouldFilterByIsInvoiced()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, numFacture: 10));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();

        var invoiced = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, IsInvoiced: true, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = invoiced.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = invoiced.GetDeliveryNoteBaseInfos.Items[0].NumFacture.Should().Be(10);

        var notInvoiced = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, IsInvoiced: false, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = notInvoiced.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = notInvoiced.GetDeliveryNoteBaseInfos.Items[0].NumFacture.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldFilterByInvoiceId()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, numFacture: 10));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId, numFacture: 20));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, IsInvoiced: true, null, InvoiceId: 10, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].NumFacture.Should().Be(10);
    }

    [Fact]
    public async Task Handle_ShouldFilterByTechnicianId()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, technicianId: 5));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null, null, TechnicianId: 5, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldFilterByTagIds()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 10, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 20, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();

        var tag = new Tag { Name = "Urgent" };
        _ = context.Tag.Add(tag);
        _ = context.SaveChanges();
        _ = context.DocumentTag.Add(new DocumentTag
        {
            TagId = tag.Id,
            DocumentType = DocumentTypes.BonDeLivraison,
            DocumentId = 10
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null, null, null,
                TagIds: new List<int> { tag.Id }),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(10);

        var noMatch = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null, null, null,
                TagIds: new List<int> { 999 }),
            CancellationToken.None);

        _ = noMatch.GetDeliveryNoteBaseInfos.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchKeyword_OnCustomerName()
    {
        var (context, yearId, handler) = Setup();
        var client1 = CreateClient(1, "Alpha Corp");
        var client2 = CreateClient(2, "Beta Inc");
        _ = context.Client.Add(client1);
        _ = context.Client.Add(client2);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 2, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null,
                SearchKeyword: "Alpha", null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].CustomerName.Should().Be("Alpha Corp");
    }

    [Fact]
    public async Task Handle_ShouldSortByNumberAscending()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 30, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 10, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(3, 20, new DateTime(2024, 6, 3), 1, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "Number",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Select(i => i.Number)
            .Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByNumberDescending()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 10, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 30, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(3, 20, new DateTime(2024, 6, 3), 1, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Descending, SortProperty: "Number",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Select(i => i.Number)
            .Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByDate()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 3), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(3, 3, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();

        var asc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "Date",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = asc.GetDeliveryNoteBaseInfos.Items.Select(i => i.Date.Date)
            .Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Descending, SortProperty: "Date",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = desc.GetDeliveryNoteBaseInfos.Items.Select(i => i.Date.Date)
            .Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByCustomerName()
    {
        var (context, yearId, handler) = Setup();
        var clientA = CreateClient(1, "Alpha");
        var clientB = CreateClient(2, "Beta");
        _ = context.Client.Add(clientA);
        _ = context.Client.Add(clientB);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 2, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();

        var asc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "CustomerName",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = asc.GetDeliveryNoteBaseInfos.Items[0].CustomerName.Should().Be("Alpha");
        _ = asc.GetDeliveryNoteBaseInfos.Items[1].CustomerName.Should().Be("Beta");

        var desc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Descending, SortProperty: "CustomerName",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = desc.GetDeliveryNoteBaseInfos.Items[0].CustomerName.Should().Be("Beta");
        _ = desc.GetDeliveryNoteBaseInfos.Items[1].CustomerName.Should().Be("Alpha");
    }

    [Fact]
    public async Task Handle_ShouldSortByNetAmount()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, net: 200));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId, net: 100));
        _ = context.SaveChanges();

        var asc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "NetAmount",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = asc.GetDeliveryNoteBaseInfos.Items.Select(i => i.NetAmount)
            .Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Descending, SortProperty: "NetAmount",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = desc.GetDeliveryNoteBaseInfos.Items.Select(i => i.NetAmount)
            .Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByGrossAmount()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, gross: 200));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId, gross: 100));
        _ = context.SaveChanges();

        var asc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "GrossAmount",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = asc.GetDeliveryNoteBaseInfos.Items.Select(i => i.GrossAmount)
            .Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByVatAmount()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId, vat: 26));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId, vat: 13));
        _ = context.SaveChanges();

        var asc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "VatAmount",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = asc.GetDeliveryNoteBaseInfos.Items.Select(i => i.VatAmount)
            .Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldSortByStatut()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        var dn1 = CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId);
        var dn2 = CreateDeliveryNote(2, 2, new DateTime(2024, 6, 2), 1, yearId);
        dn2.Valider();
        _ = context.BonDeLivraison.Add(dn1);
        _ = context.BonDeLivraison.Add(dn2);
        _ = context.SaveChanges();

        var asc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "Statut",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = asc.GetDeliveryNoteBaseInfos.Items.Select(i => i.Statut)
            .Should().BeInAscendingOrder();

        var desc = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Descending, SortProperty: "Statut",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = desc.GetDeliveryNoteBaseInfos.Items.Select(i => i.Statut)
            .Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldNotThrow_WhenSortPropertyUnknown()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 2, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 1, new DateTime(2024, 6, 2), 1, yearId));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null,
                SortOrder: SortConstants.Ascending, SortProperty: "UnknownField",
                null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldCombineMultipleFilters()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Target");
        _ = context.Client.Add(client);
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(
            1, 10, new DateTime(2024, 6, 15), 1, yearId, numFacture: 5));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(
            2, 20, new DateTime(2024, 5, 1), 1, yearId));
        _ = context.SaveChanges();

        var tag = new Tag { Name = "Filtered" };
        _ = context.Tag.Add(tag);
        _ = context.SaveChanges();
        _ = context.DocumentTag.Add(new DocumentTag
        {
            TagId = tag.Id,
            DocumentType = DocumentTypes.BonDeLivraison,
            DocumentId = 10
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                PageNumber: 1, PageSize: 10, IsInvoiced: true,
                CustomerId: 1, InvoiceId: 5,
                SortOrder: SortConstants.Ascending, SortProperty: "Number",
                SearchKeyword: "Target",
                StartDate: new DateTime(2024, 6, 1),
                EndDate: new DateTime(2024, 6, 30),
                Status: (int)DocumentStatus.Draft,
                TechnicianId: null,
                TagIds: new List<int> { tag.Id }),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(10);
        _ = result.TotalGrossAmount.Should().Be(100);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyActiveAccountingYearData()
    {
        var (context, yearId, handler) = Setup();
        var client = CreateClient(1, "Client");
        _ = context.Client.Add(client);

        var year2025 = AccountingYear.CreateAccountingYear(2025, false);
        _ = context.AccountingYear.Add(year2025);
        _ = context.SaveChanges();

        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1, 1, new DateTime(2024, 6, 1), 1, yearId));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2, 2, new DateTime(2025, 1, 1), 1, year2025.Id));
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetDeliveryNotesBaseInfosWithSummariesQuery(
                1, 10, null, null, null, null, null, null, null, null, null, null, null),
            CancellationToken.None);

        _ = result.GetDeliveryNoteBaseInfos.Items.Should().HaveCount(1);
        _ = result.GetDeliveryNoteBaseInfos.Items[0].Number.Should().Be(1);
    }
}
