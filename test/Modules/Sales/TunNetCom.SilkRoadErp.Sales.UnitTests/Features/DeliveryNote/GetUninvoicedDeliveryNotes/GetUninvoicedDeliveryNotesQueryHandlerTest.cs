using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNotes;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNote.GetUninvoicedDeliveryNotes
{
    public class GetUninvoicedDeliveryNotesQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly TestLogger<GetUninvoicedDeliveryNotesQueryHandler> _testLogger;
        private readonly GetUninvoicedDeliveryNotesQueryHandler _handler;
        public GetUninvoicedDeliveryNotesQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _testLogger = new TestLogger<GetUninvoicedDeliveryNotesQueryHandler>();
            _handler = new GetUninvoicedDeliveryNotesQueryHandler(_context, _testLogger);
        }

        [Fact]
        public async Task Handle_NoUninvoicedDeliveryNotes_ReturnsEmptyList()
        {
            // Arrange
            int clientId = 1;
            _ = _context.BonDeLivraison.Add(new BonDeLivraison
            {
                Num = 1,
                ClientId = clientId,
                NumFacture = 123, // déjà facturé
                Date = DateTime.Today
            });
            _ = await _context.SaveChangesAsync();
            var query = new GetUninvoicedDeliveryNotesQuery(clientId);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().BeEmpty();
            _ = _testLogger.Logs.Should().Contain(log => log.Contains($"Getting BonDeLivraisons with customer id {clientId}"));
            _ = _testLogger.Logs.Should().Contain(log => log.Contains("Fetched 0 BonDeLivraison"));
        }

        [Fact]
        public async Task Handle_UninvoicedDeliveryNotesExist_ReturnsMappedList()
        {
            // Arrange
            int clientId = 2;
            _context.BonDeLivraison.AddRange(
                new BonDeLivraison { Num = 2, ClientId = clientId, NumFacture = null, Date = DateTime.Today },
                new BonDeLivraison { Num = 3, ClientId = clientId, NumFacture = null, Date = DateTime.Today }
            );
            _ = await _context.SaveChangesAsync();
            var query = new GetUninvoicedDeliveryNotesQuery(clientId);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().HaveCount(2);
            _ = result.Value.Should().AllBeOfType<DeliveryNoteResponse>();
        }

        [Fact]
        public async Task Handle_LogsCorrectMessages()
        {
            // Arrange
            int clientId = 5;
            _ = _context.BonDeLivraison.Add(new BonDeLivraison
            {
                Num = 5,
                ClientId = clientId,
                NumFacture = null,
                Date = DateTime.Today
            });
            _ = await _context.SaveChangesAsync();
            var query = new GetUninvoicedDeliveryNotesQuery(clientId);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = _testLogger.Logs.Should().Contain(log => log.Contains($"Getting BonDeLivraisons with customer id {clientId}"));
            _ = _testLogger.Logs.Should().Contain(log => log.Contains("Fetched 1 BonDeLivraison"));
        }
    }
}
