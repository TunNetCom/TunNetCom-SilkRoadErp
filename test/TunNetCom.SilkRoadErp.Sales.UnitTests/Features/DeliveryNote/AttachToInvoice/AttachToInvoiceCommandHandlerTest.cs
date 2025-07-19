using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNotes
{
    public class AttachToInvoiceCommandHandlerTest
    {
        private readonly SalesContext _context;
        private readonly AttachToInvoiceCommandHandler _handler;
        private readonly Mock<ILogger<AttachToInvoiceCommandHandler>> _loggerMock;
        public AttachToInvoiceCommandHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<AttachToInvoiceCommandHandler>>();
            _handler = new AttachToInvoiceCommandHandler(_context, _loggerMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenInvoiceNotFound()
        {
            // Arrange
            var command = new AttachToInvoiceCommand(InvoiceId: 9999, DeliveryNoteIds: new List<int> { 1, 2 });
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message == "not_found");
        }
    }
}
