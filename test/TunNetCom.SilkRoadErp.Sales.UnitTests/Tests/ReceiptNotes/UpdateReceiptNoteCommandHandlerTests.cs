namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;

public class UpdateReceiptNoteCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<UpdateReceiptNoteCommandHandler> _testlogger;
    private readonly UpdateReceiptNoteCommandHandler _handler;

    public UpdateReceiptNoteCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
        _context = new SalesContext(options);
        _testlogger = new TestLogger<UpdateReceiptNoteCommandHandler>();
        _handler = new UpdateReceiptNoteCommandHandler(_context, _testlogger);
    }

    [Fact]
    public async Task Handle_ReceiptNoteNotFound_ReturnFailResult()
    {
        //Arrange
        var command = new UpdateReceiptNoteCommand(
            Num: 12345,
            NumBonFournisseur: 12345,
            DateLivraison: new DateTime(2020, 20, 20),
            IdFournisseur: 1021,
            Date: new DateTime(2020, 20, 20),
            NumFactureFournisseur: 12345
      );
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_not_found", result.Errors.First().Message);
        Assert.Contains(_testlogger.Logs, log => log.Contains($"BonDeReception with ID: {command} not found"));
    }


    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccessResult()
    {
        //Arrange

        var receiptnote = BonDeReception.CreateReceiptNote(
            num: 12345,
            numBonFournisseur: 12345,
            dateLivraison: new DateTime(2020, 20, 20),
            idFournisseur: 1021,
            date: new DateTime(2020, 20, 20),
            numFactureFournisseur: 12345);

        _context.BonDeReception.Add(receiptnote);
        await _context.SaveChangesAsync();

        var command = new UpdateReceiptNoteCommand(
            receiptnote.Num,
            NumBonFournisseur: 12345,
            DateLivraison: new DateTime(2020, 20, 20),
            IdFournisseur: 1021,
            Date: new DateTime(2020, 20, 20),
            NumFactureFournisseur: 12345);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(_testlogger.Logs, log => log.Contains($"BonDeReception updated with ID: {receiptnote.Num} updated successfully"));
    }
}
//ReceiptNote
