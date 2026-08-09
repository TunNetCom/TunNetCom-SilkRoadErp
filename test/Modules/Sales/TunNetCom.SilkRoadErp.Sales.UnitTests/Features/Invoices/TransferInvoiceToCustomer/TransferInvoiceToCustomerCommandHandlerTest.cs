using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.TransferInvoiceToCustomer;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.TransferInvoiceToCustomer;

public class TransferInvoiceToCustomerCommandHandlerTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"TransferInvoiceToCustomerTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static TransferInvoiceToCustomerCommandHandler CreateHandler(SalesContext context)
    {
        return new TransferInvoiceToCustomerCommandHandler(
            context,
            new Mock<ILogger<TransferInvoiceToCustomerCommandHandler>>().Object);
    }

    private static Client CreateClient(int id, string name)
    {
        var client = Client.CreateClient(
            nom: name, tel: "123", adresse: "Tunis",
            matricule: $"M{id}", code: $"C{id}",
            codeCat: "CAT1", etbSec: "ES1", mail: $"{name}@test.com");
        client.SetId(id);
        return client;
    }

    private static Facture CreateDraftInvoice(int num, int clientId, int accountingYearId = 1)
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
    public async Task Handle_WhenInvoiceNotFound_ReturnsEntityNotFound()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new TransferInvoiceToCustomerCommand(InvoiceNumber: 1, TargetCustomerId: 2),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == EntityNotFound.Error().Message);
    }

    [Fact]
    public async Task Handle_WhenInvoiceHasDeliveryNotes_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Client.Add(CreateClient(2, "Beta"));
        var invoice = CreateDraftInvoice(1, clientId: 1);
        invoice.BonDeLivraison = new List<BonDeLivraison>
        {
            new()
            {
                Num = 100, Date = DateTime.UtcNow, NetPayer = 10,
                TempBl = new TimeOnly(10, 0), ClientId = 1, AccountingYearId = 1
            }
        };
        _ = context.Facture.Add(invoice);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new TransferInvoiceToCustomerCommand(InvoiceNumber: 1, TargetCustomerId: 2),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "invoice_has_delivery_notes");
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotDraft_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Client.Add(CreateClient(2, "Beta"));
        var invoice = CreateDraftInvoice(1, clientId: 1);
        invoice.Valider();
        _ = context.Facture.Add(invoice);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new TransferInvoiceToCustomerCommand(InvoiceNumber: 1, TargetCustomerId: 2),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "invoice_must_be_draft");
    }

    [Fact]
    public async Task Handle_WhenTargetCustomerDoesNotExist_ReturnsEntityNotFound()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateDraftInvoice(1, clientId: 1));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new TransferInvoiceToCustomerCommand(InvoiceNumber: 1, TargetCustomerId: 999),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == EntityNotFound.Error().Message);
    }

    [Fact]
    public async Task Handle_WhenTargetCustomerIsSameAsCurrent_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Facture.Add(CreateDraftInvoice(1, clientId: 1));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new TransferInvoiceToCustomerCommand(InvoiceNumber: 1, TargetCustomerId: 1),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "target_customer_same_as_current");
    }

    [Fact]
    public async Task Handle_WhenValidTransfer_UpdatesCustomerAndReturnsSuccess()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.Client.Add(CreateClient(2, "Beta"));
        _ = context.Facture.Add(CreateDraftInvoice(1, clientId: 1));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new TransferInvoiceToCustomerCommand(InvoiceNumber: 1, TargetCustomerId: 2),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var invoice = await context.Facture.FirstAsync(f => f.Num == 1);
        _ = invoice.IdClient.Should().Be(2);
    }
}
