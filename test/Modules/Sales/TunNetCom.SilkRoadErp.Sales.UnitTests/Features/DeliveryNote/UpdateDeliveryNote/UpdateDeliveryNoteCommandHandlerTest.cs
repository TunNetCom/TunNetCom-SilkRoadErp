using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteCommandHandlerTest
{
    private readonly Mock<ILogger<UpdateDeliveryNoteCommandHandler>> _loggerMock;
    private readonly Mock<IStockCalculationService> _stockServiceMock;
    private readonly Mock<IActiveAccountingYearService> _activeYearServiceMock;

    public UpdateDeliveryNoteCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdateDeliveryNoteCommandHandler>>();
        _stockServiceMock = new Mock<IStockCalculationService>();
        _activeYearServiceMock = new Mock<IActiveAccountingYearService>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"UpdateDeliveryNote_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private UpdateDeliveryNoteCommandHandler CreateHandler(SalesContext context)
    {
        return new UpdateDeliveryNoteCommandHandler(
            context, _loggerMock.Object, _stockServiceMock.Object, _activeYearServiceMock.Object);
    }

    private static void SeedSysteme(SalesContext context, bool bloquerStock = false, bool bloquerFacture = false)
    {
        _ = context.Systeme.Add(new Systeme
        {
            NomSociete = "Test",
            Adresse = "Test",
            Tel = "123",
            CodeTva = "TVA",
            BloquerVenteStockInsuffisant = bloquerStock,
            BloquerBlSansFacture = bloquerFacture
        });
        _ = context.SaveChanges();
    }

    private static AccountingYear SeedActiveYear(SalesContext context)
    {
        var year = AccountingYear.CreateAccountingYear(2024, true);
        _ = context.AccountingYear.Add(year);
        _ = context.SaveChanges();
        return year;
    }

    private static BonDeLivraison SeedDeliveryNote(
        SalesContext context, int num, int yearId, DocumentStatus status = DocumentStatus.Draft)
    {
        var dn = BonDeLivraison.CreateBonDeLivraison(
            date: new DateTime(2024, 6, 1),
            totHTva: 100, totTva: 19, netPayer: 119,
            tempBl: new TimeOnly(10, 0),
            numFacture: null, clientId: 1, accountingYearId: yearId);
        var field = typeof(BonDeLivraison).GetField("<Num>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        field.SetValue(dn, num);
        if (status == DocumentStatus.Valid)
            dn.Valider();
        _ = context.BonDeLivraison.Add(dn);
        _ = context.SaveChanges();
        return dn;
    }

    private static void SeedLine(SalesContext context, int dnId, string refP, int qte)
    {
        _ = context.LigneBl.Add(new LigneBl
        {
            BonDeLivraisonId = dnId,
            RefProduit = refP,
            DesignationLi = refP,
            QteLi = qte,
            PrixHt = 50,
            Remise = 0,
            TotHt = 50 * qte,
            Tva = 19.0,
            TotTtc = (decimal)(50 * qte * 1.19)
        });
        _ = context.SaveChanges();
    }

    private static UpdateDeliveryNoteCommand BuildCommand(int num, params (string Ref, int Qte)[] lines)
    {
        var details = lines.Select(l => new LigneBlSubCommand
        {
            RefProduit = l.Ref, DesignationLi = l.Ref,
            QteLi = l.Qte, PrixHt = 50, Remise = 0,
            TotHt = 50 * l.Qte, Tva = 19.0, TotTtc = (decimal)(50 * l.Qte * 1.19)
        }).ToList();
        return new UpdateDeliveryNoteCommand(
            Num: num,
            Date: new DateTime(2024, 7, 1),
            TotHTva: details.Sum(d => d.TotHt),
            TotTva: details.Sum(d => d.TotTtc - d.TotHt),
            NetPayer: details.Sum(d => d.TotTtc),
            TempBl: new TimeOnly(14, 0),
            NumFacture: null, ClientId: 1,
            InstallationTechnicianId: null, DeliveryCarId: null,
            DeliveryNoteDetails: details);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenDeliveryNoteDoesNotExist()
    {
        var context = CreateContext();
        _ = SeedActiveYear(context);
        SeedSysteme(context);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = CreateHandler(context);

        var result = await handler.Handle(BuildCommand(999), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors[0].Message.Should().Be("not_found");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDeliveryNoteIsValidated()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context);
        var dn = SeedDeliveryNote(context, 1, year.Id, DocumentStatus.Valid);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        SeedLine(context, dn.Id, "REF1", 2);
        var handler = CreateHandler(context);

        var result = await handler.Handle(BuildCommand(1), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors[0].Message.Should().Be("Le document est validé et ne peut plus être modifié.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenNoActiveAccountingYear()
    {
        var context = CreateContext();
        SeedSysteme(context);
        // Seed delivery note with dummy yearId (no AccountingYear record in DB)
        _ = context.BonDeLivraison.Add(new BonDeLivraison
        {
            Num = 1,
            Date = new DateTime(2024, 6, 1),
            TotHTva = 100,
            TotTva = 19,
            NetPayer = 119,
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = 999
        });
        _ = context.SaveChanges();
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);
        var handler = CreateHandler(context);

        var result = await handler.Handle(BuildCommand(1), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors[0].Message.Should().Be("no_active_accounting_year");
    }

    [Fact]
    public async Task Handle_ShouldUpdateDeliveryNote_WhenValidCommand()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context);
        var dn = SeedDeliveryNote(context, 1, year.Id);
        SeedLine(context, dn.Id, "REF1", 2);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        _ = _stockServiceMock
            .Setup(s => s.CalculateStockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult { Reference = "REF1", StockDisponible = 100 });
        var handler = CreateHandler(context);
        var cmd = BuildCommand(1, ("REF1", 3));

        var result = await handler.Handle(cmd, CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();

        var saved = context.BonDeLivraison.Include(b => b.LigneBl).First(b => b.Num == 1);
        _ = saved.Date.Should().Be(new DateTime(2024, 7, 1));
        _ = saved.TotHTva.Should().Be(150);
        _ = saved.TotTva.Should().Be(28.5m);
        _ = saved.NetPayer.Should().Be(178.5m);
        _ = saved.TempBl.Should().Be(new TimeOnly(14, 0));
        _ = saved.AccountingYearId.Should().Be(year.Id);
        _ = saved.LigneBl.Should().HaveCount(1);
        _ = saved.LigneBl.First().RefProduit.Should().Be("REF1");
        _ = saved.LigneBl.First().QteLi.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldRemoveOldLines_AndAddNewLines()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context);
        var dn = SeedDeliveryNote(context, 1, year.Id);
        SeedLine(context, dn.Id, "REF1", 2);
        SeedLine(context, dn.Id, "REF2", 5);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        _ = _stockServiceMock
            .Setup(s => s.CalculateStockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult { Reference = "X", StockDisponible = 100 });
        var handler = CreateHandler(context);
        var cmd = BuildCommand(1, ("REF3", 1));

        var result = await handler.Handle(cmd, CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var saved = context.BonDeLivraison.Include(b => b.LigneBl).First(b => b.Num == 1);
        _ = saved.LigneBl.Should().HaveCount(1);
        _ = saved.LigneBl.First().RefProduit.Should().Be("REF3");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenBloquerBlSansFactureAndNoInvoiceNumber()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context, bloquerFacture: true);
        _ = SeedDeliveryNote(context, 1, year.Id);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        var handler = CreateHandler(context);

        var cmd = BuildCommand(1, ("REF1", 1));
        var result = await handler.Handle(cmd, CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors[0].Message.Should().Be("invoice_number_is_required");
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenBloquerBlSansFactureAndInvoiceNumberProvided()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context, bloquerFacture: true);
        var dn = SeedDeliveryNote(context, 1, year.Id);
        SeedLine(context, dn.Id, "REF1", 1);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        _ = _stockServiceMock
            .Setup(s => s.CalculateStockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult { Reference = "REF1", StockDisponible = 100 });
        var handler = CreateHandler(context);

        var cmd = new UpdateDeliveryNoteCommand(
            Num: 1, Date: new DateTime(2024, 7, 1),
            TotHTva: 100, TotTva: 19, NetPayer: 119,
            TempBl: new TimeOnly(14, 0),
            NumFacture: 5, ClientId: 1,
            InstallationTechnicianId: null, DeliveryCarId: null,
            DeliveryNoteDetails: new[]
            {
                new LigneBlSubCommand { RefProduit = "REF1", DesignationLi = "REF1", QteLi = 1, PrixHt = 100, Remise = 0, TotHt = 100, Tva = 19.0, TotTtc = 119 }
            });

        var result = await handler.Handle(cmd, CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenStockInsufficientAndBloquerStockEnabled()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context, bloquerStock: true);
        var dn = SeedDeliveryNote(context, 1, year.Id);
        SeedLine(context, dn.Id, "REF1", 2);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        _ = _stockServiceMock
            .Setup(s => s.CalculateStockAsync("REF1", year.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult { Reference = "REF1", StockDisponible = 3 });
        var handler = CreateHandler(context);

        var cmd = BuildCommand(1, ("REF1", 10));
        var result = await handler.Handle(cmd, CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors[0].Message.Should().Contain("stock_insuffisant");
    }

    [Fact]
    public async Task Handle_ShouldSucceedWithWarning_WhenStockInsufficientAndBloquerStockDisabled()
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context, bloquerStock: false);
        var dn = SeedDeliveryNote(context, 1, year.Id);
        SeedLine(context, dn.Id, "REF1", 2);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        _ = _stockServiceMock
            .Setup(s => s.CalculateStockAsync("REF1", year.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult { Reference = "REF1", StockDisponible = 3 });
        var handler = CreateHandler(context);

        var cmd = BuildCommand(1, ("REF1", 10));
        var result = await handler.Handle(cmd, CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldConsiderExistingBLQuantities_WhenCalculatingStock()
    {
        var (context, year, handler) = CreateScenario(bloquerStock: true, existingQty: 2, stockDisponible: 3);

        // réel = stockDisponible(3) + existingBL(2) = 5
        // qty 5 → 5 < 5 → false → passes (existing qty is accounted for)
        var pass = await handler.Handle(BuildCommand(1, ("REF1", 5)), CancellationToken.None);
        _ = pass.IsSuccess.Should().BeTrue();

        context.Dispose();
        (context, year, handler) = CreateScenario(bloquerStock: true, existingQty: 2, stockDisponible: 3);

        // qty 6 → 5 < 6 → true → fails (exceeds réel)
        var fail = await handler.Handle(BuildCommand(1, ("REF1", 6)), CancellationToken.None);
        _ = fail.IsFailed.Should().BeTrue();

        context.Dispose();
    }

    private (SalesContext Context, AccountingYear Year, UpdateDeliveryNoteCommandHandler Handler) CreateScenario(
        bool bloquerStock, int existingQty, int stockDisponible)
    {
        var context = CreateContext();
        var year = SeedActiveYear(context);
        SeedSysteme(context, bloquerStock: bloquerStock);
        var dn = SeedDeliveryNote(context, 1, year.Id);
        SeedLine(context, dn.Id, "REF1", existingQty);
        _ = _activeYearServiceMock
            .Setup(s => s.GetActiveAccountingYearIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(year.Id);
        _ = _stockServiceMock
            .Setup(s => s.CalculateStockAsync("REF1", year.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductStockResult { Reference = "REF1", StockDisponible = stockDisponible });
        var handler = CreateHandler(context);
        return (context, year, handler);
    }
}
