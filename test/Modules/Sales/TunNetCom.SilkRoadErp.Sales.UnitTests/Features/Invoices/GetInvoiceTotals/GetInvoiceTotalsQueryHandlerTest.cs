using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceTotals;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Invoices.GetInvoiceTotals;

public class GetInvoiceTotalsQueryHandlerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IAccountingYearFinancialParametersService> _financialParamsServiceMock;
    private readonly Mock<ILogger<GetInvoiceTotalsQueryHandler>> _loggerMock;

    public GetInvoiceTotalsQueryHandlerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _financialParamsServiceMock = new Mock<IAccountingYearFinancialParametersService>();
        _loggerMock = new Mock<ILogger<GetInvoiceTotalsQueryHandler>>();
    }

    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"GetInvoiceTotalsTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private void SetupMocks(decimal timbre = 1m, int vatRate7 = 7, int vatRate13 = 13, int vatRate19 = 19)
    {
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAppParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new GetAppParametersResponse
            {
                Timbre = timbre,
                VatRate7 = vatRate7,
                VatRate13 = vatRate13,
                VatRate19 = vatRate19
            }));

        _ = _financialParamsServiceMock
            .Setup(s => s.GetTimbreAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal fallback, CancellationToken _) => fallback);

        _ = _financialParamsServiceMock
            .Setup(s => s.GetVatRate7Async(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal fallback, CancellationToken _) => fallback);

        _ = _financialParamsServiceMock
            .Setup(s => s.GetVatRate13Async(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal fallback, CancellationToken _) => fallback);

        _ = _financialParamsServiceMock
            .Setup(s => s.GetVatRate19Async(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal fallback, CancellationToken _) => fallback);
    }

    private GetInvoiceTotalsQueryHandler CreateHandler(SalesContext context)
    {
        return new GetInvoiceTotalsQueryHandler(
            context,
            _mediatorMock.Object,
            _financialParamsServiceMock.Object,
            _loggerMock.Object);
    }

    private static LigneBl CreateLine(int idLi, string refP, decimal totHt, double tva, decimal totTtc)
    {
        return new LigneBl
        {
            IdLi = idLi,
            RefProduit = refP,
            DesignationLi = refP,
            QteLi = 1,
            PrixHt = totHt,
            Remise = 0,
            TotHt = totHt,
            Tva = tva,
            TotTtc = totTtc
        };
    }

    private static BonDeLivraison CreateDeliveryNote(int id, int num, DateTime date, int numFacture, List<LigneBl> lines)
    {
        return new BonDeLivraison
        {
            Id = id,
            Num = num,
            Date = date,
            TotHTva = lines.Sum(l => l.TotHt),
            TotTva = lines.Sum(l => l.TotTtc - l.TotHt),
            NetPayer = lines.Sum(l => l.TotTtc),
            TempBl = new TimeOnly(10, 0),
            ClientId = 1,
            AccountingYearId = 1,
            NumFacture = numFacture,
            LigneBl = lines
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnZeroTotals_WhenNoInvoicesExist()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        var result = await handler.Handle(new GetInvoiceTotalsQuery(), CancellationToken.None);

        _ = result.TotalHT.Should().Be(0);
        _ = result.TotalBase7.Should().Be(0);
        _ = result.TotalBase13.Should().Be(0);
        _ = result.TotalBase19.Should().Be(0);
        _ = result.TotalVat7.Should().Be(0);
        _ = result.TotalVat13.Should().Be(0);
        _ = result.TotalVat19.Should().Be(0);
        _ = result.TotalVat.Should().Be(0);
        _ = result.TotalTTC.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldCalculateTotals_ForSingleInvoiceWithSingleVatRate()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(new GetInvoiceTotalsQuery(), CancellationToken.None);

        _ = result.TotalHT.Should().Be(100);
        _ = result.TotalBase19.Should().Be(100);
        _ = result.TotalBase7.Should().Be(0);
        _ = result.TotalBase13.Should().Be(0);
        _ = result.TotalVat19.Should().Be(19);
        _ = result.TotalVat7.Should().Be(0);
        _ = result.TotalVat13.Should().Be(0);
        _ = result.TotalVat.Should().Be(19);
        _ = result.TotalTTC.Should().Be(119);
    }

    [Fact]
    public async Task Handle_ShouldAggregateMultipleVatRates()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 7.0, 107),
                    CreateLine(2, "B", 100, 13.0, 113),
                    CreateLine(3, "C", 100, 19.0, 119)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(new GetInvoiceTotalsQuery(), CancellationToken.None);

        _ = result.TotalHT.Should().Be(300);
        _ = result.TotalBase7.Should().Be(100);
        _ = result.TotalBase13.Should().Be(100);
        _ = result.TotalBase19.Should().Be(100);
        _ = result.TotalVat7.Should().Be(7);
        _ = result.TotalVat13.Should().Be(13);
        _ = result.TotalVat19.Should().Be(19);
        _ = result.TotalVat.Should().Be(39);
        _ = result.TotalTTC.Should().Be(339);
    }

    [Fact]
    public async Task Handle_ShouldAggregateAcrossMultipleInvoices()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 2,
            IdClient = 2,
            Date = new DateTime(2024, 6, 2),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(2, 200, new DateTime(2024, 6, 2), 2, new List<LigneBl>
                {
                    CreateLine(2, "B", 50, 7.0, 53.5m)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(new GetInvoiceTotalsQuery(), CancellationToken.None);

        _ = result.TotalHT.Should().Be(150);
        _ = result.TotalBase7.Should().Be(50);
        _ = result.TotalBase19.Should().Be(100);
        _ = result.TotalVat7.Should().Be(3.5m);
        _ = result.TotalVat19.Should().Be(19);
        _ = result.TotalVat.Should().Be(22.5m);
        _ = result.TotalTTC.Should().Be(172.5m);
    }

    [Fact]
    public async Task Handle_ShouldAddTimbreToTotalHT_PerInvoiceCount()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 1.5m);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 2,
            IdClient = 2,
            Date = new DateTime(2024, 6, 2),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(2, 200, new DateTime(2024, 6, 2), 2, new List<LigneBl>
                {
                    CreateLine(2, "B", 50, 7.0, 53.5m)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(new GetInvoiceTotalsQuery(), CancellationToken.None);

        _ = result.TotalHT.Should().Be(153); // 150 + 1.5 * 2
        _ = result.TotalVat.Should().Be(22.5m);
        _ = result.TotalTTC.Should().Be(175.5m); // 153 + 22.5
    }

    [Fact]
    public async Task Handle_ShouldNotAddTimbre_WhenInvoiceCountIsZero()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 1.5m);
        var handler = CreateHandler(context);

        var result = await handler.Handle(new GetInvoiceTotalsQuery(), CancellationToken.None);

        _ = result.TotalHT.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStartDate()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 5, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 5, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 2,
            IdClient = 2,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(2, 200, new DateTime(2024, 6, 1), 2, new List<LigneBl>
                {
                    CreateLine(2, "B", 50, 7.0, 53.5m)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetInvoiceTotalsQuery(StartDate: new DateTime(2024, 6, 1)), CancellationToken.None);

        _ = result.TotalHT.Should().Be(50);
        _ = result.TotalVat.Should().Be(3.5m);
    }

    [Fact]
    public async Task Handle_ShouldFilterByEndDateInclusive()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 2,
            IdClient = 2,
            Date = new DateTime(2024, 7, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(2, 200, new DateTime(2024, 7, 1), 2, new List<LigneBl>
                {
                    CreateLine(2, "B", 50, 7.0, 53.5m)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetInvoiceTotalsQuery(EndDate: new DateTime(2024, 6, 1)), CancellationToken.None);

        _ = result.TotalHT.Should().Be(100);
        _ = result.TotalVat.Should().Be(19);
    }

    [Fact]
    public async Task Handle_ShouldFilterByCustomerId()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 2,
            IdClient = 2,
            Date = new DateTime(2024, 6, 2),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(2, 200, new DateTime(2024, 6, 2), 2, new List<LigneBl>
                {
                    CreateLine(2, "B", 50, 7.0, 53.5m)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetInvoiceTotalsQuery(CustomerId: 1), CancellationToken.None);

        _ = result.TotalHT.Should().Be(100);
        _ = result.TotalVat.Should().Be(19);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        var draftInvoice = new Facture
        {
            Num = 1, IdClient = 1, Date = new DateTime(2024, 6, 1), AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        };
        var validInvoice = new Facture
        {
            Num = 2, IdClient = 2, Date = new DateTime(2024, 6, 2), AccountingYearId = 1
        };
        validInvoice.Valider();
        validInvoice.BonDeLivraison = new List<BonDeLivraison>
        {
            CreateDeliveryNote(2, 200, new DateTime(2024, 6, 2), 2, new List<LigneBl>
            {
                CreateLine(2, "B", 50, 7.0, 53.5m)
            })
        };
        _ = context.Facture.Add(draftInvoice);
        _ = context.Facture.Add(validInvoice);
        _ = context.SaveChanges();

        var validResult = await handler.Handle(
            new GetInvoiceTotalsQuery(Status: (int)DocumentStatus.Valid), CancellationToken.None);
        _ = validResult.TotalHT.Should().Be(50);
        _ = validResult.TotalVat.Should().Be(3.5m);

        var draftResult = await handler.Handle(
            new GetInvoiceTotalsQuery(Status: (int)DocumentStatus.Draft), CancellationToken.None);
        _ = draftResult.TotalHT.Should().Be(100);
        _ = draftResult.TotalVat.Should().Be(19);
    }

    [Fact]
    public async Task Handle_ShouldFilterByTagIds()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        });
        _ = context.SaveChanges();

        var tag = new Tag { Name = "Test" };
        _ = context.Tag.Add(tag);
        _ = context.SaveChanges();
        _ = context.DocumentTag.Add(new DocumentTag
        {
            TagId = tag.Id,
            DocumentType = DocumentTypes.Facture,
            DocumentId = 1
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(
            new GetInvoiceTotalsQuery(TagIds: new[] { tag.Id }), CancellationToken.None);
        _ = result.TotalHT.Should().Be(100);
        _ = result.TotalVat.Should().Be(19);

        var emptyResult = await handler.Handle(
            new GetInvoiceTotalsQuery(TagIds: new[] { 999 }), CancellationToken.None);
        _ = emptyResult.TotalHT.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldCombineAllFilters()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0);
        var handler = CreateHandler(context);

        var invoice = new Facture
        {
            Num = 1, IdClient = 1, Date = new DateTime(2024, 6, 15), AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 15), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 100, 19.0, 119)
                })
            }
        };
        _ = context.Facture.Add(invoice);
        _ = context.SaveChanges();

        var tag = new Tag { Name = "Important" };
        _ = context.Tag.Add(tag);
        _ = context.SaveChanges();
        _ = context.DocumentTag.Add(new DocumentTag
        {
            TagId = tag.Id,
            DocumentType = DocumentTypes.Facture,
            DocumentId = 1
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(new GetInvoiceTotalsQuery(
            StartDate: new DateTime(2024, 6, 1),
            EndDate: new DateTime(2024, 6, 30),
            CustomerId: 1,
            TagIds: new[] { tag.Id },
            Status: (int)DocumentStatus.Draft), CancellationToken.None);

        _ = result.TotalHT.Should().Be(100);
        _ = result.TotalVat.Should().Be(19);

        var noMatchResult = await handler.Handle(new GetInvoiceTotalsQuery(
            StartDate: new DateTime(2025, 1, 1),
            EndDate: new DateTime(2025, 12, 31),
            CustomerId: 1,
            TagIds: new[] { tag.Id },
            Status: (int)DocumentStatus.Draft), CancellationToken.None);

        _ = noMatchResult.TotalHT.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnSameTimbreTotalForTwoInvoices()
    {
        using var context = CreateContext();
        SetupMocks(timbre: 0.6m);
        var handler = CreateHandler(context);

        _ = context.Facture.Add(new Facture
        {
            Num = 1,
            IdClient = 1,
            Date = new DateTime(2024, 6, 1),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(1, 100, new DateTime(2024, 6, 1), 1, new List<LigneBl>
                {
                    CreateLine(1, "A", 200, 19.0, 238)
                })
            }
        });
        _ = context.Facture.Add(new Facture
        {
            Num = 2,
            IdClient = 1,
            Date = new DateTime(2024, 6, 2),
            AccountingYearId = 1,
            BonDeLivraison = new List<BonDeLivraison>
            {
                CreateDeliveryNote(2, 200, new DateTime(2024, 6, 2), 2, new List<LigneBl>
                {
                    CreateLine(2, "B", 300, 19.0, 357)
                })
            }
        });
        _ = context.SaveChanges();

        var result = await handler.Handle(new GetInvoiceTotalsQuery(CustomerId: 1), CancellationToken.None);

        _ = result.TotalHT.Should().Be(500 + 2 * 0.6m);
        _ = result.TotalTTC.Should().Be(result.TotalHT + result.TotalVat);
    }
}
