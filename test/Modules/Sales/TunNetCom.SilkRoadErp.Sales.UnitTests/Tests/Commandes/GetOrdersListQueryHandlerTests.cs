using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Commande
{
    public class GetOrdersListQueryHandlerTests : IDisposable
    {
        private readonly SalesContext _context;
        private readonly TestLogger<GetOrdersListQueryHandler> _logger;
        private readonly GetOrdersListQueryHandler _handler;

        public GetOrdersListQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _logger = new TestLogger<GetOrdersListQueryHandler>();
            _handler = new GetOrdersListQueryHandler(_context, _logger);
        }

        [Fact]
        public async Task Handle_WhenNoOrders_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetOrdersListQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task Handle_WhenOrdersExist_ReturnsCorrectSummary()
        {
            // Arrange
            var commande = new TunNetCom.SilkRoadErp.Sales.Domain.Entites.Commandes
            {
                Num = 100,
                Date = DateTime.Today,
                FournisseurId = 1,
                LigneCommandes = new List<TunNetCom.SilkRoadErp.Sales.Domain.Entites.LigneCommandes>
    {
        new() {
            NumCommande = 100,
            RefProduit = "P001",
            QteLi = 2,
            PrixHt = 50m,
            TotHt = 100m,
            Tva = 20,
            TotTtc = 120m
        }
    }
            };

            _ = _context.Commandes.Add(commande);
            _ = await _context.SaveChangesAsync();

            var query = new GetOrdersListQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var summary = Assert.Single(result.Value);
            Assert.Equal(100, summary.OrderNumber);
            Assert.Equal(100m, summary.TotalExcludingVat);
            Assert.Equal(120m, summary.NetToPay);
            Assert.Equal(20, summary.TotalVat);
        }

        public void Dispose()
        {
            _ = _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        public class TestLogger<T> : ILogger<T>
        {
            public List<string> Logs { get; } = new();

            public IDisposable BeginScope<TState>(TState state) => null!;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                Logs.Add(formatter(state, exception));
            }
        }
    }
}
