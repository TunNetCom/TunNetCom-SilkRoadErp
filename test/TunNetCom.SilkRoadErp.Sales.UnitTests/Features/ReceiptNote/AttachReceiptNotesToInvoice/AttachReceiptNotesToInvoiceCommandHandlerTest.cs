using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class AttachReceiptNotesToInvoiceCommandHandlerTests
    {
        private SalesContext CreateTestContext()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new SalesContext(options);
            return context;
        }

        [Fact]
        public async Task Handle_InvoiceNotFound_ReturnsFailure()
        {
            // Arrange
            using var context = CreateTestContext();
            var logger = Mock.Of<ILogger<AttachReceiptNotesToInvoiceCommandHandler>>();
            var handler = new AttachReceiptNotesToInvoiceCommandHandler(context, logger);
            var command = new AttachReceiptNotesToInvoiceCommand(new System.Collections.Generic.List<int> { 1 }, 999);
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e is EntityNotFound);
        }
    }
}
