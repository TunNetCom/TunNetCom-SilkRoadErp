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
            Num: 45895623,
            NumBonFournisseur: 45895623,
            DateLivraison: new DateTime(2020, 1, 20),
            IdFournisseur: 1021,
            Date: new DateTime(2020, 1, 20),
            NumFactureFournisseur: 12345
      );
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_not_found", result.Errors.First().Message);
        Assert.Contains(_testlogger.Logs, log => log.Contains($"{nameof(BonDeReception)} with ID: {command.Num} not found"));
    }


    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccessResult()
    {
        //Arrange
        var receiptnote = BonDeReception.CreateReceiptNote(
            num: 458956233,
            numBonFournisseur: 458956233,
            dateLivraison: new DateTime(2020, 1, 20),
            idFournisseur: 1,
            date: new DateTime(2020, 1, 20),
            numFactureFournisseur: 12345);

        _context.BonDeReception.Add(receiptnote);
        await _context.SaveChangesAsync();

        var command = new UpdateReceiptNoteCommand(
            receiptnote.Num,
            NumBonFournisseur: 1,
            DateLivraison: new DateTime(2020, 1, 20),
            IdFournisseur: 1,
            Date: new DateTime(2020, 1, 20),
            NumFactureFournisseur: 88);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(
            _testlogger.Logs,
            log => log.Contains($"{nameof(BonDeReception)} with ID: {receiptnote.Num} updated successfully"));
    }
}