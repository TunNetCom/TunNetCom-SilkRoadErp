using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;
public class DeleteDeliveryNoteCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly Mock<ILogger<DeleteDeliveryNoteCommandHandler>> _loggerMock;
    private readonly DeleteDeliveryNoteCommandHandler _handler;
    public DeleteDeliveryNoteCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "DeleteDeliveryNoteTestDb")
            .Options;
        _context = new SalesContext(options);
        _loggerMock = new Mock<ILogger<DeleteDeliveryNoteCommandHandler>>();
        _handler = new DeleteDeliveryNoteCommandHandler(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenDeliveryNoteExists()
    {
        // Arrange
        var deliveryNote = BonDeLivraison.CreateBonDeLivraison(
            date: System.DateTime.Today,
            totHTva: 100m,
            totTva: 20m,
            netPayer: 120m,
            tempBl: System.TimeOnly.FromDateTime(System.DateTime.Now),
            numFacture: null,
            clientId: 1);
        _ = _context.BonDeLivraison.Add(deliveryNote);
        _ = await _context.SaveChangesAsync();
        var command = new DeleteDeliveryNoteCommand(deliveryNote.Num);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        _ = _context.BonDeLivraison.Find(deliveryNote.Num).Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDeliveryNoteNotFound()
    {
        // Arrange
        var command = new DeleteDeliveryNoteCommand(9999);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "not_found");
    }


}
