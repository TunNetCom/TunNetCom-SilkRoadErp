using Xunit;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class DetachReceiptNotesFromInvoiceCommandHandlerTests : IDisposable
    {
        private readonly SalesContext _context;
        private readonly DetachReceiptNotesFromInvoiceCommandHandler _handler;
        public DetachReceiptNotesFromInvoiceCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
                .Options;
            _context = new SalesContext(options);
            var loggerMock = new Mock<ILogger<DetachReceiptNotesFromInvoiceCommandHandler>>();
            _handler = new DetachReceiptNotesFromInvoiceCommandHandler(_context, loggerMock.Object);
        }

        [Fact]
        public async Task Handle_InvoiceDoesNotExist_ReturnsFail()
        {
            var command = new DetachReceiptNotesFromInvoiceCommand(
                InvoiceId: 999, // facture inexistante
                ReceiptNoteIds: new List<int> { 1, 2 }
            );
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.False(result.IsSuccess);
            Assert.Equal("EntityNotFound", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_SuccessfulDetach_ReturnsOkAndClearsInvoiceId()
        {
            int invoiceId = 1;
            _ = _context.FactureFournisseur.Add(new FactureFournisseur { Num = invoiceId });
            _context.BonDeReception.AddRange(
                new BonDeReception { Num = 10, NumFactureFournisseur = invoiceId },
                new BonDeReception { Num = 20, NumFactureFournisseur = invoiceId }
            );
            _ = await _context.SaveChangesAsync();
            var command = new DetachReceiptNotesFromInvoiceCommand(
                InvoiceId: invoiceId,
                ReceiptNoteIds: new List<int> { 10, 20 }
            );
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.True(result.IsSuccess);
            var updatedNotes = await _context.BonDeReception
                .Where(r => command.ReceiptNoteIds.Contains(r.Num))
                .ToListAsync();
            Assert.All(updatedNotes, note => Assert.Null(note.NumFactureFournisseur));
        }
        public void Dispose() => _context.Dispose();
    }

}


