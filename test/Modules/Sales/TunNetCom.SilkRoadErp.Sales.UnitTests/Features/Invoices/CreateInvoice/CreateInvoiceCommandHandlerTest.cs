using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.CreateInvoice;

public class CreateInvoiceCommandHandlerTest
{
    private readonly Mock<ILogger<CreateInvoiceCommandHandler>> _loggerMock;
    private readonly Mock<INumberGeneratorService> _numberGeneratorMock;

    public CreateInvoiceCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreateInvoiceCommandHandler>>();
        _numberGeneratorMock = new Mock<INumberGeneratorService>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"CreateInvoiceTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private CreateInvoiceCommandHandler CreateHandler(SalesContext context)
    {
        return new CreateInvoiceCommandHandler(context, _loggerMock.Object, _numberGeneratorMock.Object);
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

    private static int SeedActiveYear(SalesContext context)
    {
        var year = AccountingYear.CreateAccountingYear(2024, true);
        _ = context.AccountingYear.Add(year);
        _ = context.SaveChanges();
        return year.Id;
    }

    [Fact]
    public async Task Handle_WhenClientDoesNotExist_ReturnsNotFoundFailure()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new CreateInvoiceCommand(DateTime.Today, ClientId: 999),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "not_found");
        _numberGeneratorMock.Verify(
            s => s.GenerateFactureNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoActiveAccountingYear_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.AccountingYear.Add(AccountingYear.CreateAccountingYear(2024, false));
        _ = context.SaveChanges();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new CreateInvoiceCommand(DateTime.Today, ClientId: 1),
            CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "no_active_accounting_year");
        _numberGeneratorMock.Verify(
            s => s.GenerateFactureNumberAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenClientExistsAndActiveYear_GeneratesNumberAndCreatesInvoice()
    {
        using var context = CreateContext();
        var activeYearId = SeedActiveYear(context);
        _ = context.Client.Add(CreateClient(1, "Alpha"));
        _ = context.SaveChanges();
        _ = _numberGeneratorMock
            .Setup(s => s.GenerateFactureNumberAsync(activeYearId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(202400001);

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new CreateInvoiceCommand(new DateTime(2024, 6, 1), ClientId: 1),
            CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.Value.Should().Be(202400001);
        _numberGeneratorMock.Verify(
            s => s.GenerateFactureNumberAsync(activeYearId, It.IsAny<CancellationToken>()),
            Times.Once);

        var invoice = context.Facture.Single();
        _ = invoice.Num.Should().Be(202400001);
        _ = invoice.IdClient.Should().Be(1);
        _ = invoice.AccountingYearId.Should().Be(activeYearId);
        _ = invoice.Date.Should().Be(new DateTime(2024, 6, 1));
    }
}
