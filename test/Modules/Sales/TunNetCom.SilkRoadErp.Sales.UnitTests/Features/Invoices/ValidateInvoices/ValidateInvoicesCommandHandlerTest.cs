using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.ValidateInvoices;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.ValidateInvoices;

public class ValidateInvoicesCommandHandlerTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"ValidateInvoicesTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static ValidateInvoicesCommandHandler CreateHandler(SalesContext context)
    {
        return new ValidateInvoicesCommandHandler(
            context,
            new Mock<ILogger<ValidateInvoicesCommandHandler>>().Object);
    }

    private static Facture CreateInvoice(int num, int clientId = 1, int accountingYearId = 1)
    {
        return new Facture
        {
            Num = num,
            IdClient = clientId,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = accountingYearId
        };
    }

    [Fact]
    public async Task Handle_WhenNoIdsProvided_ReturnsFailure()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(new ValidateInvoicesCommand(new List<int>()), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "No ids provided");
    }

    [Fact]
    public async Task Handle_WhenNullIdsProvided_ReturnsFailure()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(new ValidateInvoicesCommand(null!), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "No ids provided");
    }

    [Fact]
    public async Task Handle_WhenSomeInvoicesMissing_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = context.Facture.Add(CreateInvoice(1));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateInvoicesCommand(new List<int> { 1, 2 }), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "invoices_not_found: 2");
    }

    [Fact]
    public async Task Handle_WhenAllDraft_ValidatesAndReturnsSuccess()
    {
        using var context = CreateContext();
        _ = context.Facture.Add(CreateInvoice(1));
        _ = context.Facture.Add(CreateInvoice(2));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateInvoicesCommand(new List<int> { 1, 2 }), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var invoices = await context.Facture.OrderBy(f => f.Num).ToListAsync();
        _ = invoices.Should().OnlyContain(i => i.Statut == DocumentStatus.Valid);
    }

    [Fact]
    public async Task Handle_WhenAlreadyValid_SkipsAndReturnsSuccess()
    {
        using var context = CreateContext();
        var invoice = CreateInvoice(1);
        invoice.Valider();
        _ = context.Facture.Add(invoice);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateInvoicesCommand(new List<int> { 1 }), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var stored = await context.Facture.FirstAsync(f => f.Num == 1);
        _ = stored.Statut.Should().Be(DocumentStatus.Valid);
    }

    [Fact]
    public async Task Handle_WhenMixedDraftAndValid_ValidatesOnlyDraft()
    {
        using var context = CreateContext();
        var draft = CreateInvoice(1);
        var valid = CreateInvoice(2);
        valid.Valider();
        _ = context.Facture.Add(draft);
        _ = context.Facture.Add(valid);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateInvoicesCommand(new List<int> { 1, 2 }), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var invoices = await context.Facture.OrderBy(f => f.Num).ToListAsync();
        _ = invoices.Should().OnlyContain(i => i.Statut == DocumentStatus.Valid);
    }
}
