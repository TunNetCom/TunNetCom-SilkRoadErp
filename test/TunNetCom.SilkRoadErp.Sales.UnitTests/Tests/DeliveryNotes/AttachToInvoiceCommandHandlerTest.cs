//namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNotes;

//public class AttachToInvoiceCommandHandlerTest
//{
//    private readonly SalesContext _context;
//    private readonly TestLogger<AttachToInvoiceCommandHandler> _testLogger;
//    private readonly AttachToInvoiceCommandHandler _attachToInvoiceCommandHandler;

//    public AttachToInvoiceCommandHandlerTest()
//    {
//        var options = new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(databaseName: "SalesContext")
//            .Options;
//        _context = new SalesContext(options);
//        _testLogger = new TestLogger<AttachToInvoiceCommandHandler>();
//        _attachToInvoiceCommandHandler = new AttachToInvoiceCommandHandler(_context, _testLogger);
//    }

//    [Fact]
//    public async Task Handle_DeliveryNotesNotFound_ReturnsFailResult()
//    {
//        // Arrange
//        var command = new AttachToInvoiceCommand(
//            InvoiceId: 202512,
//            DeliveryNoteIds: new List<int> { 202501, 202502, 202503 } 
//        );
//        var invoice = new Facture { Num = 202512, IdClient = 100 };
//        _context.Facture.Add(invoice);
//        await _context.SaveChangesAsync();

//        // Act
//        var result = await _attachToInvoiceCommandHandler.Handle(command, CancellationToken.None);
//        // Assert
//        Assert.Equal("not_found", result.Errors.First().Message);
//        //TODO add fluent assertions if it's free
//        Assert.False(result.IsSuccess);
//    }

//    [Fact]
//    public async Task Handle_DeliveryNotesAlreadyAttached_ReturnsFailResult()
//    {
//        // Arrange
//        var invoice = new Facture { Num = 202512, IdClient = 100 };
//        _context.Facture.Add(invoice);

//        var deliveryNotes = new List<BonDeLivraison>
//        {
//            new BonDeLivraison { Num = 202501, ClientId = 100, NumFacture = 2 },
//            new BonDeLivraison { Num = 202502, ClientId = 100, NumFacture = null }
//        };
//        _context.BonDeLivraison.AddRange(deliveryNotes);
//        await _context.SaveChangesAsync();

//        var command = new AttachToInvoiceCommand(
//            InvoiceId: 202512,
//            DeliveryNoteIds: new List<int> { 20251, 202502 }
//        );

//        // Act
//        var result = await _attachToInvoiceCommandHandler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.False(result.IsSuccess);
//        Assert.Equal("delivery_note_already_attached", result.Errors.First().Message);
//        Assert.Contains(_testLogger.Logs, log => log.Contains("Delivery notes 1 are already attached to invoices 2"));
//    }

//    [Fact]
//    public async Task Handle_ValidAttachment_ReturnsSuccessResult()
//    {
//        // Arrange
//        var invoice = new Facture { Num = 202512, IdClient = 100 };
//        _context.Facture.Add(invoice);

//        var deliveryNotes = new List<BonDeLivraison>
//        {
//            new BonDeLivraison { Num = 202501, ClientId = 100, NumFacture = null },
//            new BonDeLivraison { Num = 202502, ClientId = 100, NumFacture = null }
//        };
//        _context.BonDeLivraison.AddRange(deliveryNotes);
//        await _context.SaveChangesAsync();

//        var command = new AttachToInvoiceCommand(
//            InvoiceId: 202512,
//            DeliveryNoteIds: new List<int> { 202501, 202502 }
//        );

//        // Act
//        var result = await _attachToInvoiceCommandHandler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.True(result.IsSuccess);
//        Assert.Contains(_testLogger.Logs, log => log.Contains("Successfully attached delivery notes 1, 2 to invoice 1"));
//    }
//}