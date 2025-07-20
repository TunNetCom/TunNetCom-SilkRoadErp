using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;
public class GetFullInvoiceByIdQueryHandlerTest
{
    private readonly SalesContext _context;
    private readonly GetFullInvoiceByIdQueryHandler _handler;
    private readonly Mock<ILogger<GetFullInvoiceByIdQueryHandler>> _mockLogger;
    public GetFullInvoiceByIdQueryHandlerTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new SalesContext(options);
        _mockLogger = new Mock<ILogger<GetFullInvoiceByIdQueryHandler>>();
        _handler = new GetFullInvoiceByIdQueryHandler(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvoice_WhenInvoiceExists()
    {
        // Arrange
        var client = Client.CreateClient(
            nom: "Client 1",
            tel: "123456",
            adresse: "Tunis",
            matricule: "MA123",
            code: "C1",
            codeCat: "CAT1",
            etbSec: "ES1",
            mail: "test@client.com"
        );
        var facture = new Facture
        {
            Num = 100,
            IdClient = client.Id,
            Date = DateTime.Today,
            IdClientNavigation = client,
            BonDeLivraison = new List<BonDeLivraison>
            {
                new BonDeLivraison
                {
                    Num = 200,
                    Date = DateTime.Today,
                    TotHTva = 100,
                    TotTva = 20,
                    NetPayer = 120,
                    TempBl = new TimeOnly(14, 30),
                    ClientId = client.Id,
                    LigneBl = new List<LigneBl>
                    {
                        new LigneBl
                        {
                            IdLi = 1,
                            RefProduit = "PRD01",
                            DesignationLi = "Produit 1",
                            QteLi = 2,
                            PrixHt = 50,
                            Remise = 0,
                            TotHt = 100,
                            Tva = 20,
                            TotTtc = 120
                        }
                    }
                }
            }
        };
        _context.Client.Add(client);
        _context.Facture.Add(facture);
        await _context.SaveChangesAsync();
        var query = new GetFullInvoiceByIdQuery(facture.Num);
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(facture.Num, result.Value.Num);
        Assert.Equal(client.Id, result.Value.IdClient);
        Assert.NotEmpty(result.Value.DeliveryNotes);
        Assert.Equal("Client 1", result.Value.Client.Nom); 
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenInvoiceNotFound()
    {
        // Arrange
        var query = new GetFullInvoiceByIdQuery(999); 
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
       // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("not_found", result.Errors[0].Message); 
    }
}
