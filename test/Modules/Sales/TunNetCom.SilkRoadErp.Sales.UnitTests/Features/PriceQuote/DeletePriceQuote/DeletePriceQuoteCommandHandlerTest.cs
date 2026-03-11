using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.DeletePriceQuote;
public class DeletePriceQuoteCommandHandlerTest
{
    private readonly DbContextOptions<SalesContext> _dbContextOptions;
    private readonly Mock<ILogger<DeletePriceQuoteCommandHandler>> _loggerMock;
    public DeletePriceQuoteCommandHandlerTest()
    {
        _dbContextOptions = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "DeletePriceQuoteTestDb")
            .Options;
        _loggerMock = new Mock<ILogger<DeletePriceQuoteCommandHandler>>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenDevisExistsAndIsDeleted()
    {
        // Arrange
        using var context = new SalesContext(_dbContextOptions);
        var existingDevis = Devis.CreateDevis(1, 10, DateTime.UtcNow, 100, 19, 119);
        _ = context.Devis.Add(existingDevis);
        _ = await context.SaveChangesAsync();
        var handler = new DeletePriceQuoteCommandHandler(context, _loggerMock.Object);
        var command = new DeletePriceQuoteCommand(1);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        _ = context.Devis.Find(1).Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDevisDoesNotExist()
    {
        // Arrange
        using var context = new SalesContext(_dbContextOptions);
        var handler = new DeletePriceQuoteCommandHandler(context, _loggerMock.Object);
        var command = new DeletePriceQuoteCommand(999); // Num non existant
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message.Contains("not_found"));
    }
}
