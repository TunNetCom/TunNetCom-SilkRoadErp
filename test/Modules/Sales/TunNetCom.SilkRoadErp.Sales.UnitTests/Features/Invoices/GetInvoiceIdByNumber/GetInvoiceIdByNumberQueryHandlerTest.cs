using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceIdByNumber;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.GetInvoiceIdByNumber;

public class GetInvoiceIdByNumberQueryHandlerTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"GetInvoiceIdByNumberTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static GetInvoiceIdByNumberQueryHandler CreateHandler(SalesContext context)
    {
        return new GetInvoiceIdByNumberQueryHandler(
            context,
            new Mock<ILogger<GetInvoiceIdByNumberQueryHandler>>().Object);
    }

    [Fact]
    public async Task Handle_WhenInvoiceExists_ReturnsInvoiceId()
    {
        using var context = CreateContext();
        _ = context.Facture.Add(new Facture
        {
            Num = 123,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1
        });
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetInvoiceIdByNumberQuery(123), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenInvoiceDoesNotExist_ReturnsFailure()
    {
        using var context = CreateContext();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetInvoiceIdByNumberQuery(999), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "invoice_not_found");
    }

    [Fact]
    public async Task Handle_WhenMultipleInvoices_ReturnsMatchingNumber()
    {
        using var context = CreateContext();
        _ = context.Facture.Add(new Facture { Num = 1, IdClient = 1, Date = DateTime.UtcNow, AccountingYearId = 1 });
        _ = context.Facture.Add(new Facture { Num = 2, IdClient = 2, Date = DateTime.UtcNow, AccountingYearId = 1 });
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new GetInvoiceIdByNumberQuery(2), CancellationToken.None);

        _ = result.Value.Should().Be(2);
    }
}
