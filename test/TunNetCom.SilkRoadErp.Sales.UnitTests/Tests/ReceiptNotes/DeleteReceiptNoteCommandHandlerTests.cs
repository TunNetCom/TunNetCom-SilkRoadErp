namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;

public class DeleteReceiptNoteCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<DeleteReceiptNoteCommandHandler> _testlogger;
    private readonly DeleteReceiptNoteCommandHandler _handler;

    public DeleteReceiptNoteCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
            _context = new SalesContext(options);
            _testlogger = new TestLogger<DeleteReceiptNoteCommandHandler>();
            _handler = new DeleteReceiptNoteCommandHandler(_context, _testlogger);
    }

    [Fact]
    public async Task Handle_ReceiptNoteNotFound_ReturnError()
    {
        //Arrange
        var command = new DeleteReceiptNoteCommand(num: 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_not_found", result.Errors.First().Message);
        Assert.Contains(_testlogger.Logs, log => log.Contains($"BonDeReception with ID: {command} not found"));
    }

    [Fact]
    public async Task Handle_Logs_ReceiptNoteDeleted()
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
        var command = new DeleteReceiptNoteCommand(num: receiptnote.Num);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.Contains(_testlogger.Logs, log => log.Contains($"Delete BonDeReception with values: {command}"));
    }

    [Fact]
    public async Task Handle_ReceiptNoteDeleted_ReturnSuccess()
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
        var command = new DeleteReceiptNoteCommand(num: receiptnote.Num);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_context.BonDeReception, r => r.Num == receiptnote.Num);
    }

}
