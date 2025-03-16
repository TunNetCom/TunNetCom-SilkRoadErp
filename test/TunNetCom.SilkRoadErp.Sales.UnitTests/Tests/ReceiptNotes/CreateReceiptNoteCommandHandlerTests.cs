namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;

public class CreateReceiptNoteCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<CreateReceiptNoteCommandHandler> _testlogger;
    private readonly CreateReceiptNoteCommandHandler _handler;

    public CreateReceiptNoteCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
        _context = new SalesContext(options);
        _testlogger = new TestLogger<CreateReceiptNoteCommandHandler>();
        _handler = new CreateReceiptNoteCommandHandler(_context, _testlogger);
    }

    [Fact]
    public async Task Handle_ReceiptNoteNumberExists_ReturnsFailResult()
    {
        // Arrange
        var receiptnote = BonDeReception.CreateReceiptNote(
            num: 1234567822,
            numBonFournisseur: 1234567822,
            dateLivraison: new DateTime(2020, 11, 20),
            idFournisseur: 1,
            date: new DateTime(2020, 11, 20),
            numFactureFournisseur: 12345);
        
        _context.BonDeReception.Add(receiptnote);
        await _context.SaveChangesAsync();

        var command = new CreateReceiptNoteCommand(
            Num: 1234567822,
            NumBonFournisseur: 1234567822,
            DateLivraison: new DateTime(2020, 11, 20),
            IdFournisseur: 1,
            Date: new DateTime(2020, 11, 20),
            NumFactureFournisseur: 12345);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_number_exists", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_CreateReceiptNote__ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateReceiptNoteCommand(
          Num: 12345123,
          NumBonFournisseur: 12345123,
          DateLivraison: new DateTime(2020, 11, 20),
          IdFournisseur: 1,
          Date: new DateTime(2020, 11, 20),
          NumFactureFournisseur: 12345);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_LogsReceiptNoteCreated()
    {
        // Arrange
        var command = new CreateReceiptNoteCommand(
          Num: 123456,
          NumBonFournisseur: 123456,
          DateLivraison: new DateTime(2020, 4, 20),
          IdFournisseur: 1,
          Date: new DateTime(2020, 4, 20),
          NumFactureFournisseur: 12345);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(_testlogger.Logs, log => log.Contains($"Creating BonDeReception with values: {command}"));
    }

    [Fact]
    public async Task Handle_LogsReceiptNoteCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateReceiptNoteCommand(
          Num: 123459,
          NumBonFournisseur: 123459,
          DateLivraison: new DateTime(2020, 7, 20),
          IdFournisseur: 1,
          Date: new DateTime(2020, 7, 20),
          NumFactureFournisseur: 12345);

        var receiptnote = BonDeReception.CreateReceiptNote(
           num: 123459,
           numBonFournisseur: 123459,
           dateLivraison: new DateTime(2020, 7, 20),
           idFournisseur: 1,
           date: new DateTime(2020, 7, 20),
           numFactureFournisseur: 12345);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Expected operation to succeed");
        Assert.Contains(_testlogger.Logs, log => log.Contains($"{nameof(BonDeReception)} created successfully with ID: {result.Value}"));
    }
}
