using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteCommandHandlerTestcontainersTests : IClassFixture<SqlServerTestcontainerFixture>, IAsyncLifetime
{
    private readonly SqlServerTestcontainerFixture _fixture;

    public CreateDeliveryNoteCommandHandlerTestcontainersTests(SqlServerTestcontainerFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => _fixture.InitializeAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static void SeedSysteme(SalesContext context)
    {
        if (context.Systeme.Any())
            return;
        context.Systeme.Add(new Systeme
        {
            NomSociete = "Test",
            Adresse = "Test",
            Tel = "123",
            CodeTva = "TVA",
            BloquerVenteStockInsuffisant = false,
            BloquerBlSansFacture = false
        });
        context.SaveChanges();
    }

    private static AccountingYear SeedActiveAccountingYear(SalesContext context)
    {
        var year = AccountingYear.CreateAccountingYear(2024, isActive: true);
        context.AccountingYear.Add(year);
        context.SaveChanges();
        return year;
    }

    [Fact]
    public async Task Handle_WhenNoActiveAccountingYear_ReturnsFailResult()
    {
        await using var context = _fixture.CreateContext();
        SeedSysteme(context);
        // Do not seed AccountingYear so there is no active year

        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        var stockMock = new Mock<IStockCalculationService>();
        var activeYearMock = new Mock<IActiveAccountingYearService>();
        activeYearMock.Setup(x => x.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);

        var logger = new TestLogger<CreateDeliveryNoteCommandHandler>();
        var handler = new CreateDeliveryNoteCommandHandler(
            context,
            logger,
            numberGeneratorMock.Object,
            stockMock.Object,
            activeYearMock.Object);

        var command = new CreateDeliveryNoteCommand(
            Date: DateTime.Today,
            TotHTva: 100m,
            TotTva: 19m,
            NetPayer: 119m,
            TempBl: TimeOnly.FromDateTime(DateTime.Now),
            NumFacture: null,
            ClientId: null,
            InstallationTechnicianId: null,
            DeliveryCarId: null,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("no_active_accounting_year", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CreatesDeliveryNoteInSqlServer()
    {
        await using var context = _fixture.CreateContext();
        SeedSysteme(context);
        var accountingYear = SeedActiveAccountingYear(context);

        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        numberGeneratorMock
            .Setup(x => x.GenerateBonDeLivraisonNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var stockMock = new Mock<IStockCalculationService>();
        stockMock
            .Setup(x => x.CalculateStockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult
            {
                Reference = "REF1",
                StockDisponible = 100,
                StockCalcule = 100
            });

        var activeYearMock = new Mock<IActiveAccountingYearService>();
        activeYearMock
            .Setup(x => x.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountingYear.Id);

        var logger = new TestLogger<CreateDeliveryNoteCommandHandler>();
        var handler = new CreateDeliveryNoteCommandHandler(
            context,
            logger,
            numberGeneratorMock.Object,
            stockMock.Object,
            activeYearMock.Object);

        var command = new CreateDeliveryNoteCommand(
            Date: new DateTime(2024, 6, 15),
            TotHTva: 100m,
            TotTva: 19m,
            NetPayer: 119m,
            TempBl: new TimeOnly(10, 0),
            NumFacture: null,
            ClientId: null,
            InstallationTechnicianId: null,
            DeliveryCarId: null,
            DeliveryNoteDetails: new[]
            {
                new LigneBlSubCommand
                {
                    RefProduit = "REF1",
                    DesignationLi = "Product 1",
                    QteLi = 2,
                    QteLivree = 2,
                    PrixHt = 50m,
                    Remise = 0,
                    TotHt = 100m,
                    Tva = 19,
                    TotTtc = 119m
                }
            });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);

        var saved = await context.BonDeLivraison
            .Include(b => b.LigneBl)
            .FirstOrDefaultAsync(b => b.Num == 1);
        Assert.NotNull(saved);
        Assert.Equal(1, saved.Num);
        Assert.Equal(accountingYear.Id, saved.AccountingYearId);
        Assert.Equal(100m, saved.TotHTva);
        Assert.Equal(19m, saved.TotTva);
        Assert.Equal(119m, saved.NetPayer);
        var line = Assert.Single(saved.LigneBl);
        Assert.Equal("REF1", line.RefProduit);
        Assert.Equal(2, line.QteLi);
    }
}
