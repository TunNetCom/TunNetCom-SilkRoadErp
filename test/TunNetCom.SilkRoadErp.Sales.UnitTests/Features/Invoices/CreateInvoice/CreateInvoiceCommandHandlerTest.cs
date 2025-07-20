using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.CreateInvoice
{
    public class CreateInvoiceCommandHandlerTests
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<CreateInvoiceCommandHandler>> _loggerMock;
        private readonly CreateInvoiceCommandHandler _handler;
        public CreateInvoiceCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<CreateInvoiceCommandHandler>>();
            _handler = new CreateInvoiceCommandHandler(_context, _loggerMock.Object);
        }
        [Fact]
        public async Task Handle_ClientDoesNotExist_ReturnsFail()
        {
            var command = new CreateInvoiceCommand(DateTime.Today, ClientId: 999);
            var result = await _handler.Handle(command, CancellationToken.None);
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("not_found");
        }

        [Fact]
        public async Task Handle_ClientExists_CreatesInvoiceAndReturnsNum()
        {
            var client = Client.CreateClient(
                nom: "Test Client",
                tel: "123456",
                adresse: "Test Address",
                matricule: "M123",
                code: "C001",
                codeCat: "CAT1",
                etbSec: "ES1",
                mail: "test@example.com");
            _context.Client.Add(client);
            await _context.SaveChangesAsync();
            var command = new CreateInvoiceCommand(DateTime.Today, client.Id);
            var result = await _handler.Handle(command, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
           var invoiceInDb = await _context.Facture.FirstOrDefaultAsync(f => f.Num == result.Value);
            invoiceInDb.Should().NotBeNull();
            invoiceInDb!.IdClient.Should().Be(client.Id);
        }

        [Fact]
        public async Task Handle_LogsInformationMessages()
        {
            var client = Client.CreateClient(
                nom: "Test Client",
                tel: "123456",
                adresse: "Test Address",
                matricule: "M123",
                code: "C001",
                codeCat: "CAT1",
                etbSec: "ES1",
                mail: "test@example.com");
            _context.Client.Add(client);
            await _context.SaveChangesAsync();
            var command = new CreateInvoiceCommand(DateTime.Today, client.Id);
            var result = await _handler.Handle(command, CancellationToken.None);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeast(2));
        }
    }
}
