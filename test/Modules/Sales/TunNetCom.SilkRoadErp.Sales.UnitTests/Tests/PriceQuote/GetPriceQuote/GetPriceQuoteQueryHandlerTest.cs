using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.PriceQuotes
{
    public class GetPriceQuoteQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly GetPriceQuoteQueryHandler _handler;
        private readonly Mock<ILogger<GetCustomerQueryHandler>> _loggerMock;
        public GetPriceQuoteQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<GetCustomerQueryHandler>>();
            _handler = new GetPriceQuoteQueryHandler(_context, _loggerMock.Object);
            SeedDatabase();
        }
        private void SeedDatabase()
        {
            var client1 = Client.CreateClient(
                nom: "Client1", tel: "123", adresse: null, matricule: null,
                code: null, codeCat: null, etbSec: null, mail: null);
            var client2 = Client.CreateClient(
                nom: "Client2", tel: "456", adresse: null, matricule: null,
                code: null, codeCat: null, etbSec: null, mail: null);
            _context.Client.AddRange(client1, client2);
            _ = _context.SaveChanges();
            _context.Devis.AddRange(
                new Devis
                {
                    Num = 1,
                    IdClient = client1.Id,
                    Date = new DateTime(2023, 01, 10),
                    TotHTva = 500,
                    TotTva = 100,
                    TotTtc = 600
                },
                new Devis
                {
                    Num = 2,
                    IdClient = client2.Id,
                    Date = new DateTime(2023, 02, 20),
                    TotHTva = 800,
                    TotTva = 160,
                    TotTtc = 960
                }
            );
            _ = _context.SaveChanges();
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedQuotations_WithoutSearch()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, null);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredQuotations_WithSearch()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, "1");
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().ContainSingle();
            _ = result.Items[0].IdClient.Should().Be(1);
        }
        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenSearchDoesNotMatch()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, "XYZ999"); // valeur qui ne correspond à rien
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().BeEmpty();
        }
    }
}
