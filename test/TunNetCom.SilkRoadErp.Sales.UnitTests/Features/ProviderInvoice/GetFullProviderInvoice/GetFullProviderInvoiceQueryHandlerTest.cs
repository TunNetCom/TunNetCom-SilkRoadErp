using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetFullProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using Xunit;

public class GetFullProviderInvoiceQueryHandlerTests
{
    private readonly SalesContext _context;
    private readonly Mock<ILogger<GetFullProviderInvoiceQueryHandler>> _loggerMock;
    private readonly GetFullProviderInvoiceQueryHandler _handler;

    public GetFullProviderInvoiceQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB
            .Options;

        _context = new SalesContext(options);
        _loggerMock = new Mock<ILogger<GetFullProviderInvoiceQueryHandler>>();
        _handler = new GetFullProviderInvoiceQueryHandler(_context, _loggerMock.Object);

        SeedData();
    }

    private void SeedData()
    {
        var fournisseur = new Fournisseur
        {
            Id = 1,
            Nom = "Supplier A",
            Tel = "123456",
            Adresse = "123 Street",
            Matricule = "MAT123",
            Code = "SUP001",
            CodeCat = "CAT01",
            EtbSec = "ETB02",
            Mail = "supplier@mail.com"
        };

        _context.Fournisseur.Add(fournisseur);

        var ligne1 = new LigneBonReception
        {
            IdLigne = 1,
            RefProduit = "P001",
            DesignationLi = "Produit 1",
            QteLi = 10,
            PrixHt = 100,
            Remise = 5,
            TotHt = 950,
            Tva = 19,
            TotTtc = 1130.5m
        };

        var bonReception = new BonDeReception
        {
            Num = 1,
            Date = DateTime.Today,
            IdFournisseur = 1,
            LigneBonReception = new List<LigneBonReception> { ligne1 }
        };

        _context.BonDeReception.Add(bonReception);

        var facture = new FactureFournisseur
        {
            Num = 999,
            IdFournisseur = 1,
            Date = DateTime.Today,
            BonDeReception = new List<BonDeReception> { bonReception }
        };

        _context.FactureFournisseur.Add(facture);

        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ShouldReturnFullProviderInvoice_WhenInvoiceExists()
    {
        // Arrange
        var query = new GetFullProviderInvoiceQuery(999);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(999, result.Value.ProviderInvoiceNumber);
        Assert.Single(result.Value.ReceiptNotes);
        Assert.Equal("P001", result.Value.ReceiptNotes.First().Lines.First().ProductReference);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenInvoiceDoesNotExist()
    {
        // Arrange
        var query = new GetFullProviderInvoiceQuery(404);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("not_found", result.Errors.First().Message); // correction ici
    }

}
