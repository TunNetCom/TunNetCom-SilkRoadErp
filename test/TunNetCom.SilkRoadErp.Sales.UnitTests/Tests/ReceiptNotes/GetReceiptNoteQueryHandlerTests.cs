using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;

public class GetReceiptNoteQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetReceiptNoteQueryHandler> _testlogger;
    private readonly GetReceiptNoteQueryHandler _handler;

    public GetReceiptNoteQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
        _context = new SalesContext(options);
        _testlogger = new TestLogger<GetReceiptNoteQueryHandler>();
        _handler = new GetReceiptNoteQueryHandler(_context, _testlogger);
    }

    [Fact]
    public async Task Handle_PaginationRequest_LogsPagination()
    {
        // Arrange
        var query = new GetReceiptNoteQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: null
        );

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Contains(
            _testlogger.Logs,
            log => log.Contains($"Fetching {nameof(BonDeReception)} with pageIndex: {query.PageNumber} and pageSize: {query.PageSize}"));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_EmptySearchKeyword_ReturnsAllReceiptNotes()
    {
        var receiptnote1 = BonDeReception.CreateReceiptNote(
            num: 123456662,
            numBonFournisseur: 123456662,
            dateLivraison: new DateTime(2020, 1, 20),
            idFournisseur: 1,
            date: new DateTime(2020, 1, 20),
            numFactureFournisseur: 12345);

        var receiptnote2 = BonDeReception.CreateReceiptNote(
            num: 123456696,
            numBonFournisseur: 123456696,
            dateLivraison: new DateTime(2020, 1, 20),
            idFournisseur: 1,
            date: new DateTime(2020, 1, 20),
            numFactureFournisseur: 12345);

        _context.BonDeReception.Add(receiptnote1);
        _context.BonDeReception.Add(receiptnote2);
        await _context.SaveChangesAsync();

        var query = new GetReceiptNoteQuery(
           PageNumber: 1,
           PageSize: 10,
           SearchKeyword: ""
       );

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Count > 2);
    }
}
