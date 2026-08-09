using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.UpdateInvoiceDate;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.UpdateInvoiceDate;

public class UpdateInvoiceDateCommandHandlerTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"UpdateInvoiceDateTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static UpdateInvoiceDateCommandHandler CreateHandler(SalesContext context)
    {
        return new UpdateInvoiceDateCommandHandler(
            context,
            new Mock<ILogger<UpdateInvoiceDateCommandHandler>>().Object);
    }

    private static Facture CreateDraftInvoice(int num)
    {
        return new Facture
        {
            Num = num,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1
        };
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotFound_ReturnsEntityNotFound()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new UpdateInvoiceDateCommand(Num: 999, Date: DateTime.UtcNow),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == EntityNotFound.Error().Message);
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotDraft_ReturnsFailure()
    {
        using var context = CreateContext();
        var invoice = CreateDraftInvoice(1);
        invoice.Valider();
        _ = context.Facture.Add(invoice);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new UpdateInvoiceDateCommand(Num: 1, Date: DateTime.UtcNow),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "invoice_date_only_draft");
    }

    [Fact]
    public async Task Handle_WhenDraftInvoice_UpdatesDateAndReturnsSuccess()
    {
        using var context = CreateContext();
        _ = context.Facture.Add(CreateDraftInvoice(1));
        _ = context.SaveChanges();

        var newDate = new DateTime(2024, 7, 15);
        var handler = CreateHandler(context);
        var result = await handler.Handle(new UpdateInvoiceDateCommand(Num: 1, Date: newDate), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var invoice = await context.Facture.FirstOrDefaultAsync(f => f.Num == 1);
        _ = invoice!.Date.Should().Be(newDate);
    }
}
