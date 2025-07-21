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
            _context.Devis.AddRange(new List<Devis>
            {
                new Devis { Num = 1, IdClient = 100, Date = new System.DateTime(2024, 1, 1), TotHTva = 100, TotTva = 19, TotTtc = 119 },
                new Devis { Num = 2, IdClient = 101, Date = new System.DateTime(2024, 2, 2), TotHTva = 200, TotTva = 38, TotTtc = 238 },
                new Devis { Num = 3, IdClient = 102, Date = new System.DateTime(2024, 3, 3), TotHTva = 300, TotTva = 57, TotTtc = 357 }
            });
            _context.SaveChanges();
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
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.Items.Should().Contain(q => q.Num == 1);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredList_WhenSearchKeywordMatches()
        {
            // Arrange
            var query = new GetPriceQuoteQuery(1, 10, "101"); 
            var handler = new GetPriceQuoteQueryHandler(_context, _loggerMock.Object);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().IdClient.Should().Be(101);
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
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }
    }
}
