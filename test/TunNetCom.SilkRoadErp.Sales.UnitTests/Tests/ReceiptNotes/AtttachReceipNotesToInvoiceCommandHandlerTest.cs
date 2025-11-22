using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class AttachReceiptNotesToInvoiceCommandHandlerTest
    {
        private SalesContext CreateTestContext()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new SalesContext(options);

            // IMPORTANT: Comme ProviderInvoiceView n'a pas de clé primaire, EF Core le configure par défaut sans clé.
            // Ici tu peux simuler les données via une autre entité avec clé si besoin pour les autres tests.
            // Pour ce test simple, on ne met rien car on teste la non-existence.

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
