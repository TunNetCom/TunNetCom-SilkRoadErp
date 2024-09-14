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
        Assert.Contains(_testlogger.Logs, log => log.Contains($"Fetching BonDeReception with pageIndex: {query.PageNumber} and pageSize: {query.PageSize}"));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_SearchKeyword_FiltersReceiptNotes()
    {
        // Arrange
        var query = new GetReceiptNoteQuery(
          PageNumber: 1,
          PageSize: 10,
          SearchKeyword: "12345"
      );

        var receiptnote = BonDeReception.CreateReceiptNote(
            num: 12345,
            numBonFournisseur: 12345,
            dateLivraison: new DateTime(2020, 20, 20),
            idFournisseur: 1021,
            date: new DateTime(2020, 20, 20),
            numFactureFournisseur: 12345);

        _context.BonDeReception.Add(receiptnote);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(12345, result.First().Num);
    }

    [Fact]
    public async Task Handle_EmptySearchKeyword_ReturnsAllReceiptNotes()
    {
        var receiptnote1 = BonDeReception.CreateReceiptNote(
            num: 12345,
            numBonFournisseur: 12345,
            dateLivraison: new DateTime(2020, 20, 20),
            idFournisseur: 1021,
            date: new DateTime(2020, 20, 20),
            numFactureFournisseur: 12345);

        var receiptnote2 = BonDeReception.CreateReceiptNote(
            num: 12345,
            numBonFournisseur: 12345,
            dateLivraison: new DateTime(2020, 20, 20),
            idFournisseur: 1021,
            date: new DateTime(2020, 20, 20),
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
        Assert.Equal(2, result.Count);
    }
}
