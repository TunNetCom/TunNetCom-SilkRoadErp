using Microsoft.Extensions.Logging.Abstractions;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Views;
using Xunit;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class AttachReceiptNotesToInvoiceCommandHandlerTest
    {
        private SalesContext CreateTestContext()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SalesContext(options);
        }

        [Fact]
        public async Task Handle_InvoiceNotFound_ReturnsFailure()
        {
            // Arrange
            using var context = CreateTestContext();
            var logger = Mock.Of<ILogger<AttachReceiptNotesToInvoiceCommandHandler>>();
            var handler = new AttachReceiptNotesToInvoiceCommandHandler(context, logger);
            var command = new AttachReceiptNotesToInvoiceCommand([1], 999);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}

