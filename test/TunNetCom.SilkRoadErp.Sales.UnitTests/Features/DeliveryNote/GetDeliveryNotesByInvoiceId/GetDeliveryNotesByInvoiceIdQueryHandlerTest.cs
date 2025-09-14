using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByInvoiceId;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNote
{
    public class GetDeliveryNotesByInvoiceIdQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly TestLogger<GetDeliveryNotesByInvoiceIdQueryHandler> _testLogger;
        private readonly GetDeliveryNotesByInvoiceIdQueryHandler _handler;
        public GetDeliveryNotesByInvoiceIdQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _testLogger = new TestLogger<GetDeliveryNotesByInvoiceIdQueryHandler>();
            _handler = new GetDeliveryNotesByInvoiceIdQueryHandler(_context, _testLogger);
        }

        [Fact]
        public async Task Handle_NoDeliveryNotesFound_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetDeliveryNotesByInvoiceIdQuery(123);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
            Assert.Contains(_testLogger.Logs, log => log.Contains("Retreiving BonDeLivraison with invoice id"));
            Assert.Contains(_testLogger.Logs, log => log.Contains("Fetched 0 BonDeLivraison"));
        }

        [Fact]
        public async Task Handle_DeliveryNotesExist_ReturnsMappedList()
        {
            // Arrange
            var deliveryNote1 = new BonDeLivraison { Num = 1, NumFacture = 500, Date = DateTime.Today };
            var deliveryNote2 = new BonDeLivraison { Num = 2, NumFacture = 500, Date = DateTime.Today };
            _context.BonDeLivraison.AddRange(deliveryNote1, deliveryNote2);
            _ = await _context.SaveChangesAsync();
            var query = new GetDeliveryNotesByInvoiceIdQuery(500);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.All(result.Value, item => Assert.IsType<DeliveryNoteResponse>(item));
        }

        [Fact]
        public async Task Handle_LogsCorrectMessages()
        {
            // Arrange
            var deliveryNote = new BonDeLivraison { Num = 10, NumFacture = 888, Date = DateTime.Today };
            _ = _context.BonDeLivraison.Add(deliveryNote);
            _ = await _context.SaveChangesAsync();
            var query = new GetDeliveryNotesByInvoiceIdQuery(888);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains(_testLogger.Logs, log => log.Contains("Retreiving BonDeLivraison with invoice id : 888"));
            Assert.Contains(_testLogger.Logs, log => log.Contains("Fetched 1 BonDeLivraison"));
        }
    }
}
