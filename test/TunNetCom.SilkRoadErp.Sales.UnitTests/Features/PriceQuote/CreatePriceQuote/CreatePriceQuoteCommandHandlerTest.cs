using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Quotations
{
    public class CreatePriceQuoteCommandHandlerTest
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<CreatePriceQuoteCommandHandler>> _loggerMock;

        public CreatePriceQuoteCommandHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _context.Database.EnsureCreated();
            _loggerMock = new Mock<ILogger<CreatePriceQuoteCommandHandler>>();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenQuotationDoesNotExist()
        {
            // Arrange
            var handler = new CreatePriceQuoteCommandHandler(_context, _loggerMock.Object);
            var command = new CreatePriceQuoteCommand(
                Num: 1001,
                IdClient: 1,
                Date: DateTime.UtcNow,
                TotHTva: 100,
                TotTva: 19,
                TotTtc: 119
            );
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1001, result.Value);
            var created = await _context.Devis.FirstOrDefaultAsync(d => d.Num == 1001);
            Assert.NotNull(created);
            Assert.Equal(1, created.IdClient);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenQuotationAlreadyExists()
        {
            // Arrange: Add a quotation with Num = 2001
            _context.Devis.Add(Devis.CreateDevis(2001, 2, DateTime.UtcNow, 100, 20, 120));
            await _context.SaveChangesAsync();
            var handler = new CreatePriceQuoteCommandHandler(_context, _loggerMock.Object);
            var command = new CreatePriceQuoteCommand(
                Num: 2001, // Same number
                IdClient: 2,
                Date: DateTime.UtcNow,
                TotHTva: 200,
                TotTva: 40,
                TotTtc: 240
            );
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("quotations_num_exist", result.Errors[0].Message);
        }
    }
}
