using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DetachFromInvoice;
public class DetachFromInvoiceCommandHandlerTest
{
    private SalesContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"SalesDb_{System.Guid.NewGuid()}")
            .Options;
        var context = new SalesContext(options);
        _ = context.Facture.Add(new Facture { Num = 1 }); 
        context.BonDeLivraison.AddRange(
            new BonDeLivraison { Num = 101, NumFacture = 1 },
            new BonDeLivraison { Num = 102, NumFacture = 1 },
            new BonDeLivraison { Num = 103, NumFacture = 2 } 
        );
        _ = context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenInvoiceExistsAndNotesDetached()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var loggerMock = new Mock<ILogger<DetachFromInvoiceCommandHandler>>();
        var handler = new DetachFromInvoiceCommandHandler(context, loggerMock.Object);
        var request = new DetachFromInvoiceCommand(
            InvoiceId: 1,
            DeliveryNoteIds: new List<int> { 101, 102 }
        );
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        var detachedNotes = await context.BonDeLivraison
            .ToListAsync();
        _ = detachedNotes
            .FindAll(n => request.DeliveryNoteIds.Contains(n.Num))
            .Should()
            .OnlyContain(n => n.NumFacture == null);
    }
    [Fact]
    public async Task Handle_ShouldReturnFail_WhenInvoiceNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext(); 
        var loggerMock = new Mock<ILogger<DetachFromInvoiceCommandHandler>>();
        var handler = new DetachFromInvoiceCommandHandler(context, loggerMock.Object);
        var request = new DetachFromInvoiceCommand(
            InvoiceId: 999,
            DeliveryNoteIds: new List<int> { 101, 102 }
        );
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors[0].Message.Should().Be("invoice_not_found");
    }
}
