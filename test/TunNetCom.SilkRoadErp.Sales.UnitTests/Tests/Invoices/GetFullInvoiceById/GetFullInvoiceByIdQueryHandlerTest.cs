
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.GetFullInvoiceByldTest
{
    public class GetFullInvoiceByIdQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly TestLogger<GetFullInvoiceByIdQueryHandler> _logger;
        private readonly GetFullInvoiceByIdQueryHandler getFullInvoiceByIdQueryHandler;
        public GetFullInvoiceByIdQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "SalesContext")
                .Options;
            _context = new SalesContext(options);
            _logger = new TestLogger<GetFullInvoiceByIdQueryHandler>();
            getFullInvoiceByIdQueryHandler = new GetFullInvoiceByIdQueryHandler(_context, _logger);
        }
        [Fact]
        public async Task Handle_ValidInvoiceId_ReturnsFullInvoice()
        {
            // Arrange
            var client = Client.CreateClient("Test", "123", "Rue X", "MAT", "CODE", "CAT", "ETB", "mail@test.com");
            _context.Client.Add(client);
            await _context.SaveChangesAsync();

            var invoice = new Facture
            {
                Date = DateTime.Now,
                IdClient = client.Id,
                IdClientNavigation = client
            };
            _context.Facture.Add(invoice);
            await _context.SaveChangesAsync();

            var query = new GetFullInvoiceByIdQuery(invoice.Num);

            // Act
            var result = await getFullInvoiceByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(invoice.Num, result.Value.Num);
            Assert.Equal(client.Nom, result.Value.Client.Nom);
        }
        [Fact]
        public async Task Handle_InvalidInvoiceId_ReturnsNotFound()
        {
            // Arrange
            var query = new GetFullInvoiceByIdQuery(999); // Invoice inexistante

            // Act
            var result = await getFullInvoiceByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("not_found", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_LogsInvoiceNotFound()
        {
            // Arrange
            var query = new GetFullInvoiceByIdQuery(999);

            // Act
            var _ = await getFullInvoiceByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains($"Facture including BonDeLivraisons, LigneBls and Client with ID: {query.Id} not found"));
        }
    }
}
