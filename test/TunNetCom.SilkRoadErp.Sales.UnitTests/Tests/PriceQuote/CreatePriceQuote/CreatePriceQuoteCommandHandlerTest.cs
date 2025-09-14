using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;

public class CreatePriceQuoteCommandHandlerTest
{
    private readonly SalesContext _context;
    private readonly Mock<ILogger<CreatePriceQuoteCommandHandler>> _loggerMock;
    private readonly CreatePriceQuoteCommandHandler _handler;
    public CreatePriceQuoteCommandHandlerTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new SalesContext(options);
        _loggerMock = new Mock<ILogger<CreatePriceQuoteCommandHandler>>();
        _handler = new CreatePriceQuoteCommandHandler(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenQuotationNumAlreadyExists()
    {
        // Arrange
        var existingDevis = Devis.CreateDevis(123, 1, DateTime.UtcNow, 100m, 20m, 120m);
        _ = _context.Devis.Add(existingDevis);
        _ = await _context.SaveChangesAsync();
        var command = new CreatePriceQuoteCommand(
            Num: 123,      // même numéro que existant
            IdClient: 1,
            Date: DateTime.UtcNow,
            TotHTva: 100m,
            TotTva: 20m,
            TotTtc: 120m
        );
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "quotations_num_exist");
    }

    [Fact]
    public async Task Handle_ShouldCreateDevisAndReturnNum_WhenNewQuotation()
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: 999,
            IdClient: 42,
            Date: DateTime.UtcNow,
            TotHTva: 200m,
            TotTva: 40m,
            TotTtc: 240m
        );
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().Be(999);
        var createdDevis = await _context.Devis.FirstOrDefaultAsync(d => d.Num == 999);
        _ = createdDevis.Should().NotBeNull();
        _ = createdDevis.IdClient.Should().Be(42);
        _ = createdDevis.TotHTva.Should().Be(200m);
    }
}
