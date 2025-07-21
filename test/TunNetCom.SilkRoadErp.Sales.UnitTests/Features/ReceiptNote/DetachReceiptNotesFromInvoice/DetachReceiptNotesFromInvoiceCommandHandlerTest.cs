using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class DetachReceiptNotesFromInvoiceCommandHandlerTest : IDisposable
    {
        private readonly SalesContext _context;
        private readonly DetachReceiptNotesFromInvoiceCommandHandler _handler;
        private readonly Mock<ILogger<DetachReceiptNotesFromInvoiceCommandHandler>> _loggerMock;
        public DetachReceiptNotesFromInvoiceCommandHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<DetachReceiptNotesFromInvoiceCommandHandler>>();
            _handler = new DetachReceiptNotesFromInvoiceCommandHandler(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var command = new DetachReceiptNotesFromInvoiceCommand(
                InvoiceId: 999, // Invoice non existante
                ReceiptNoteIds: new List<int> { 1, 2 }
            );
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("EntityNotFound", result.Errors.First().Message);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldDetachReceiptNotes_WhenInvoiceExists()
        {
            // Arrange
            var invoiceId = 123;
            _context.FactureFournisseur.Add(new FactureFournisseur { Num = invoiceId });
            _context.BonDeReception.AddRange(
                new BonDeReception { Num = 1, NumFactureFournisseur = invoiceId },
                new BonDeReception { Num = 2, NumFactureFournisseur = invoiceId },
                new BonDeReception { Num = 3, NumFactureFournisseur = 999 }
            );
            await _context.SaveChangesAsync();
            var command = new DetachReceiptNotesFromInvoiceCommand(
                InvoiceId: invoiceId,
                ReceiptNoteIds: new List<int> { 1, 2, 3 }
            );
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            var updatedNotes = await _context.BonDeReception
                .Where(br => new[] { 1, 2, 3 }.Contains(br.Num))
                .ToListAsync();
            Assert.All(updatedNotes.Where(r => r.Num == 1 || r.Num == 2), r =>
                Assert.Null(r.NumFactureFournisseur));
            Assert.Equal(999, updatedNotes.First(r => r.Num == 3).NumFactureFournisseur); 
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully detached")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        public void Dispose() => _context.Dispose();
    }
}
