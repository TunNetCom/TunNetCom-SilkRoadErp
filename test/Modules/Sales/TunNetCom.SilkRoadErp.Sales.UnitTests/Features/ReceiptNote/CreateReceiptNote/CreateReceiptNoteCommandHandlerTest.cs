using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.ReceiptNote.CreateReceiptNote;

public class CreateReceiptNoteCommandHandlerTest : IDisposable
{
    private static SalesContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SalesContext(options);
    }

    private static Fournisseur CreateSeededFournisseur()
    {
        return new Fournisseur
        {
            Id = 1,
            Nom = "Test Fournisseur",
            Tel = "12345678",
            Fax = "Fax Test",
            Matricule = "Mat123",
            Code = "Code001",
            CodeCat = "Cat01",
            EtbSec = "Sec01",
            Mail = "fournisseur@example.com"
        };
    }

    private static AccountingYear CreateSeededActiveAccountingYear()
    {
        return AccountingYear.CreateAccountingYear(2024, isActive: true);
    }

    [Fact]
    public async Task Handle_WhenProviderNotFound_ReturnsFailResult()
    {
        using var context = CreateTestContext();
        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        numberGeneratorMock
            .Setup(x => x.GenerateBonDeReceptionNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var testLogger = new TestLogger<CreateReceiptNoteCommandHandler>();
        var handler = new CreateReceiptNoteCommandHandler(context, testLogger, numberGeneratorMock.Object);

        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 123,
            DateLivraison: DateTime.Today,
            IdFournisseur: 999,
            Date: DateTime.Today,
            NumFactureFournisseur: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("not_found", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_WhenNoActiveAccountingYear_ReturnsFailResult()
    {
        using var context = CreateTestContext();
        var fournisseur = CreateSeededFournisseur();
        context.Fournisseur.Add(fournisseur);
        await context.SaveChangesAsync();

        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        numberGeneratorMock
            .Setup(x => x.GenerateBonDeReceptionNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var testLogger = new TestLogger<CreateReceiptNoteCommandHandler>();
        var handler = new CreateReceiptNoteCommandHandler(context, testLogger, numberGeneratorMock.Object);

        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 123,
            DateLivraison: DateTime.Today,
            IdFournisseur: fournisseur.Id,
            Date: DateTime.Today,
            NumFactureFournisseur: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("no_active_accounting_year", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CreatesReceiptNoteAndReturnsId()
    {
        using var context = CreateTestContext();
        var fournisseur = CreateSeededFournisseur();
        var accountingYear = CreateSeededActiveAccountingYear();
        context.Fournisseur.Add(fournisseur);
        context.AccountingYear.Add(accountingYear);
        await context.SaveChangesAsync();

        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        const int generatedNum = 1;
        numberGeneratorMock
            .Setup(x => x.GenerateBonDeReceptionNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(generatedNum);
        var testLogger = new TestLogger<CreateReceiptNoteCommandHandler>();
        var handler = new CreateReceiptNoteCommandHandler(context, testLogger, numberGeneratorMock.Object);

        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 456,
            DateLivraison: new DateTime(2024, 5, 10),
            IdFournisseur: fournisseur.Id,
            Date: new DateTime(2024, 5, 10),
            NumFactureFournisseur: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var createdId = result.Value;
        Assert.True(createdId > 0);

        var receiptNote = await context.BonDeReception.FirstOrDefaultAsync(b => b.Id == createdId);
        Assert.NotNull(receiptNote);
        Assert.Equal(generatedNum, receiptNote.Num);
        Assert.Equal(command.NumBonFournisseur, receiptNote.NumBonFournisseur);
        Assert.Equal(command.IdFournisseur, receiptNote.IdFournisseur);
        Assert.Equal(accountingYear.Id, receiptNote.AccountingYearId);
        Assert.Equal(command.DateLivraison, receiptNote.DateLivraison);
        Assert.Equal(command.Date, receiptNote.Date);
        Assert.Equal(command.NumFactureFournisseur, receiptNote.NumFactureFournisseur);
    }

    [Fact]
    public async Task Handle_LogsEntityCreated()
    {
        using var context = CreateTestContext();
        var fournisseur = CreateSeededFournisseur();
        var accountingYear = CreateSeededActiveAccountingYear();
        context.Fournisseur.Add(fournisseur);
        context.AccountingYear.Add(accountingYear);
        await context.SaveChangesAsync();

        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        numberGeneratorMock
            .Setup(x => x.GenerateBonDeReceptionNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var testLogger = new TestLogger<CreateReceiptNoteCommandHandler>();
        var handler = new CreateReceiptNoteCommandHandler(context, testLogger, numberGeneratorMock.Object);

        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 456,
            DateLivraison: DateTime.Today,
            IdFournisseur: fournisseur.Id,
            Date: DateTime.Today,
            NumFactureFournisseur: null);

        _ = await handler.Handle(command, CancellationToken.None);

        Assert.Contains(testLogger.Logs, log => log.Contains(nameof(BonDeReception)) && log.Contains("Creating"));
    }

    [Fact]
    public async Task Handle_LogsEntityCreatedSuccessfully()
    {
        using var context = CreateTestContext();
        var fournisseur = CreateSeededFournisseur();
        var accountingYear = CreateSeededActiveAccountingYear();
        context.Fournisseur.Add(fournisseur);
        context.AccountingYear.Add(accountingYear);
        await context.SaveChangesAsync();

        var numberGeneratorMock = new Mock<INumberGeneratorService>();
        numberGeneratorMock
            .Setup(x => x.GenerateBonDeReceptionNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var testLogger = new TestLogger<CreateReceiptNoteCommandHandler>();
        var handler = new CreateReceiptNoteCommandHandler(context, testLogger, numberGeneratorMock.Object);

        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 456,
            DateLivraison: DateTime.Today,
            IdFournisseur: fournisseur.Id,
            Date: DateTime.Today,
            NumFactureFournisseur: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(testLogger.Logs, log =>
            log.Contains(nameof(BonDeReception)) && log.Contains("created successfully") && log.Contains(result.Value.ToString()));
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
