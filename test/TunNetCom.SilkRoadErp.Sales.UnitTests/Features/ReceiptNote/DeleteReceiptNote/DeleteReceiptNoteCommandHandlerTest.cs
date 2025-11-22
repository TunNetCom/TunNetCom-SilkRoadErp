//namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;
//public class DeleteReceiptNoteCommandHandlerTest
//{
//    private readonly SalesContext _context;
//    private readonly TestLogger<DeleteReceiptNoteCommandHandler> _testLogger;
//    private readonly DeleteReceiptNoteCommandHandler _handler;
//    public DeleteReceiptNoteCommandHandlerTest()
//    {
//        var options = new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(Guid.NewGuid().ToString())
//            .Options;
//        _context = new SalesContext(options);
//        _testLogger = new TestLogger<DeleteReceiptNoteCommandHandler>();
//        _handler = new DeleteReceiptNoteCommandHandler(_context, _testLogger);
//    }

//    [Fact]
//    public async Task Handle_ReceiptNoteNotFound_ReturnsError()
//    {
//        // Arrange
//        var command = new DeleteReceiptNoteCommand(Num: 9999);
//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);
//        // Assert
//        _ = result.IsSuccess.Should().BeFalse();
//        _ = result.Errors.Should().ContainSingle(e => e.Message == "not_found");
//        _ = _testLogger.Logs.Should().Contain(log =>
//            log.Contains($"{nameof(BonDeReception)} with ID: {command.Num} not found"));
//    }

//    [Fact]
//    public async Task Handle_ReceiptNoteDeleted_ReturnsSuccess()
//    {
//        // Arrange
//        var receiptNote = BonDeReception.CreateReceiptNote(
//            num: 1010,
//            numBonFournisseur: 2020,
//            dateLivraison: new DateTime(2023, 1, 1),
//            idFournisseur: 1,
//            date: new DateTime(2023, 1, 1),
//            numFactureFournisseur: 5678);
//        _ = _context.BonDeReception.Add(receiptNote);
//        _ = await _context.SaveChangesAsync();
//        var command = new DeleteReceiptNoteCommand(receiptNote.Num);
//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);
//        // Assert
//        _ = result.IsSuccess.Should().BeTrue();
//        var deleted = await _context.BonDeReception.FindAsync(receiptNote.Num);
//        _ = deleted.Should().BeNull();
//    }

//    [Fact]
//    public async Task Handle_LogsReceiptNoteDeleted()
//    {
//        // Arrange
//        var receiptNote = BonDeReception.CreateReceiptNote(
//            num: 3030,
//            numBonFournisseur: 4040,
//            dateLivraison: new DateTime(2023, 5, 5),
//            idFournisseur: 2,
//            date: new DateTime(2023, 5, 5),
//            numFactureFournisseur: 7890);
//        _ = _context.BonDeReception.Add(receiptNote);
//        _ = await _context.SaveChangesAsync();
//        var command = new DeleteReceiptNoteCommand(receiptNote.Num);
//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);
//        // Assert
//        _ = _testLogger.Logs.Should().Contain(log =>
//            log.Contains($"{nameof(BonDeReception)} with ID: {command.Num} deleted successfully"));
//    }
//}
