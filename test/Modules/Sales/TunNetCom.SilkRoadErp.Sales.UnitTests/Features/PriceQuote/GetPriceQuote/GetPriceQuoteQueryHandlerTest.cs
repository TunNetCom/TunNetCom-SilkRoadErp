using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Quotations
{
    public class GetPriceQuoteQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<GetCustomerQueryHandler>> _loggerMock;

        public GetPriceQuoteQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<GetCustomerQueryHandler>>();
            SeedTestData();
        }
        private void SeedTestData()
        {
            var client1 = Client.CreateClient(nom: "Client1", tel: "1", adresse: null, matricule: null, code: null, codeCat: null, etbSec: null, mail: null);
            var client2 = Client.CreateClient(nom: "Client2", tel: "2", adresse: null, matricule: null, code: null, codeCat: null, etbSec: null, mail: null);
            var client3 = Client.CreateClient(nom: "Client3", tel: "3", adresse: null, matricule: null, code: null, codeCat: null, etbSec: null, mail: null);
            _context.Client.AddRange(client1, client2, client3);
            _ = _context.SaveChanges();
            _context.Devis.AddRange(new List<Devis>
            {
                new() { Num = 1, IdClient = client1.Id, Date = new System.DateTime(2024, 1, 1), TotHTva = 100, TotTva = 19, TotTtc = 119 },
                new() { Num = 2, IdClient = client2.Id, Date = new System.DateTime(2024, 2, 2), TotHTva = 200, TotTva = 38, TotTtc = 238 },
                new() { Num = 3, IdClient = client3.Id, Date = new System.DateTime(2024, 3, 3), TotHTva = 300, TotTva = 57, TotTtc = 357 }
            });
            _ = _context.SaveChanges();
        }
        [Fact]
        public async Task Handle_ShouldReturnPagedList_WhenNoSearchKeyword()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, null);
            var handler = new GetPriceQuoteQueryHandler(_context, _loggerMock.Object);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().HaveCount(3);
            _ = result.Items.Should().Contain(q => q.Num == 1);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredList_WhenSearchKeywordMatches()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, "1"); 
            var handler = new GetPriceQuoteQueryHandler(_context, _loggerMock.Object);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().HaveCount(1);
            _ = result.Items.First().IdClient.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenSearchKeywordNotMatches()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, "9999"); 
            var handler = new GetPriceQuoteQueryHandler(_context, _loggerMock.Object);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().BeEmpty();
        }
    }
}
