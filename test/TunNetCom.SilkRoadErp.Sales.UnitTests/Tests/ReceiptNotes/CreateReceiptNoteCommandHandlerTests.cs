using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        var command = new CreateReceiptNoteCommand(
            Num: 12345,
            NumBonFournisseur: 12345,
            DateLivraison: new DateTime(2020, 20, 20),
            IdFournisseur: 1021,
            Date: new DateTime(2020, 20, 20),
            NumFactureFournisseur: 12345
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
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("receiptnote_number_exists", result.Errors.Skip(1).First().Message);
    }

    [Fact]
    public async Task Handle_CreateReceiptNote__ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateReceiptNoteCommand(
          Num: 12345,
          NumBonFournisseur: 12345,
          DateLivraison: new DateTime(2020, 20, 20),
          IdFournisseur: 1021,
          Date: new DateTime(2020, 20, 20),
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
          Num: 12345,
          NumBonFournisseur: 12345,
          DateLivraison: new DateTime(2020, 20, 20),
          IdFournisseur: 1021,
          Date: new DateTime(2020, 20, 20),
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
          Num: 12345,
          NumBonFournisseur: 12345,
          DateLivraison: new DateTime(2020, 20, 20),
          IdFournisseur: 1021,
          Date: new DateTime(2020, 20, 20),
          NumFactureFournisseur: 12345);

        var receiptnote = BonDeReception.CreateReceiptNote(
           num: 12345,
           numBonFournisseur: 12345,
           dateLivraison: new DateTime(2020, 20, 20),
           idFournisseur: 1021,
           date: new DateTime(2020, 20, 20),
           numFactureFournisseur: 12345);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Expected operation to succeed");
        Assert.Contains(_testlogger.Logs, log => log.Contains($"BonDeReception created successfully with ID: {result.Value}"));
    }
}
