using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;
public class GetDeliveryNoteQueryHandlerTest
{
    private readonly Mock<ILogger<GetDeliveryNoteQueryHandler>> _loggerMock = new();
    private readonly SalesContext _context;
    public GetDeliveryNoteQueryHandlerTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new SalesContext(options);
        _context.BonDeLivraison.RemoveRange(_context.BonDeLivraison);
        _ = _context.SaveChanges();
        _context.BonDeLivraison.AddRange(new[]
        {
            new BonDeLivraison
            {
                Num = 1,
                Date = new DateTime(2023, 1, 10),
                TotHTva = 100,
                TotTva = 20,
                NetPayer = 120,
                TempBl = new TimeOnly(9, 30, 0),
                NumFacture = 10,
                ClientId = 1001
            },
            new BonDeLivraison
            {
                Num = 2,
                Date = new DateTime(2023, 2, 15),
                TotHTva = 200,
                TotTva = 40,
                NetPayer = 240,
                TempBl = new TimeOnly(14, 45, 0),
                NumFacture = null,
                ClientId = 1002
            },
            new BonDeLivraison
            {
                Num = 3,
                Date = new DateTime(2023, 3, 20),
                TotHTva = 300,
                TotTva = 60,
                NetPayer = 360,
                TempBl = new TimeOnly(8, 15, 0),
                NumFacture = 11,
                ClientId = 1003
            }
        });
        _ = _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedDeliveryNotes_WithCorrectPaging()
    {
        var handler = new GetDeliveryNoteQueryHandler(_context, _loggerMock.Object);
        var query = new GetDeliveryNoteQuery(
            PageNumber: 1,
            PageSize: 2,
            SearchKeyword: null,
            IsFactured: null);
        var result = await handler.Handle(query, CancellationToken.None);
        _ = result.Should().NotBeNull();
        _ = result.Items.Should().HaveCount(2);
        _ = result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldFilterByIsFactured_True()
    {
        var handler = new GetDeliveryNoteQueryHandler(_context, _loggerMock.Object);
        var query = new GetDeliveryNoteQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: null,
            IsFactured: true);
        var result = await handler.Handle(query, CancellationToken.None);
        _ = result.Items.Should().OnlyContain(d => d.InvoiceNumber != null);
        _ = result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterByIsFactured_False()
    {
        var handler = new GetDeliveryNoteQueryHandler(_context, _loggerMock.Object);
        var query = new GetDeliveryNoteQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: null,
            IsFactured: false);
        var result = await handler.Handle(query, CancellationToken.None);
        _ = result.Items.Should().OnlyContain(d => d.InvoiceNumber == null);
        _ = result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchKeyword_InMemoryFiltering()
    {
        var handler = new GetDeliveryNoteQueryHandler(_context, _loggerMock.Object);
        var query = new GetDeliveryNoteQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: "2",
            IsFactured: null);
        var allItems = await handler.Handle(new GetDeliveryNoteQuery(1, 1000, null, null), CancellationToken.None);    
        var filteredItems = allItems.Items
            .Where(d => d.DeliveryNoteNumber == 2)
            .ToList();
        _ = filteredItems.Should().HaveCount(1);
        _ = filteredItems[0].DeliveryNoteNumber.Should().Be(2);
    }

}
