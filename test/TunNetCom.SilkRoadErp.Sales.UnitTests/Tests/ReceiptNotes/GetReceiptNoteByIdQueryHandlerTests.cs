using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;

public class GetReceiptNoteByIdQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<GetReceiptNoteByIdQueryHandler> _testlogger;
    private readonly GetReceiptNoteByIdQueryHandler _handler;

    public GetReceiptNoteByIdQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
        _context = new SalesContext(options);
        _testlogger = new TestLogger<GetReceiptNoteByIdQueryHandler>();
        _handler = new GetReceiptNoteByIdQueryHandler(_context, _testlogger);
    }

    [Fact]
    public async Task Handle_ReceiptNoteNotFound_ReturnsFailResult()
    {
        // Arrange
        var query = new GetReceiptNoteByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_not_found", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ReceiptNoteFound_ReturnsReceiptNoteNote()
    {
        // Arrange
        var receiptnote = BonDeReception.CreateReceiptNote(
            num: 12345,
            numBonFournisseur: 12345,
            dateLivraison: new DateTime(2020, 20, 20),
            idFournisseur: 1021,
            date: new DateTime(2020, 20, 20),
            numFactureFournisseur: 12345);
        _context.BonDeReception.Add(receiptnote);
        await _context.SaveChangesAsync();

        var query = new GetReceiptNoteByIdQuery(receiptnote.Num);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
    [Fact]
    public async Task Handle_LogsProviderNotFound()
    {
        // Arrange
        var query = new GetReceiptNoteByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_not_found", result.Errors.First().Message);
        Assert.Contains(_testlogger.Logs, log => log.Contains($"BonDeReception with ID: {query.Num} not found"));
    }
}
