using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.DeletePriceQuote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.PriceQuotes
{
    public class DeletePriceQuoteCommandHandlerTest
    {
        private SalesContext CreateDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new SalesContext(options);
            var devis = Devis.CreateDevis(
                num: 123,
                idClient: 1,
                date: DateTime.UtcNow,
                totHTva: 100,
                totTva: 20,
                totTtc: 120);
            context.Devis.Add(devis);
            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task Handle_Should_Return_Ok_When_Devis_Exists()
        {
            // Arrange
            var context = CreateDbContextWithData();
            var loggerMock = new Mock<ILogger<DeletePriceQuoteCommandHandler>>();
            var handler = new DeletePriceQuoteCommandHandler(context, loggerMock.Object);
            var command = new DeletePriceQuoteCommand(123);
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            (await context.Devis.FindAsync(123)).Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Devis_Does_Not_Exist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new SalesContext(options); // Base vide
            var loggerMock = new Mock<ILogger<DeletePriceQuoteCommandHandler>>();
            var handler = new DeletePriceQuoteCommandHandler(context, loggerMock.Object);
            var command = new DeletePriceQuoteCommand(999); // Num non existant
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("not_found");
        }

    }
}
