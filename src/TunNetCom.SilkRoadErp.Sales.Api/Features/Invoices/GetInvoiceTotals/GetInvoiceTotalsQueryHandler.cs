using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceTotals;

public class GetInvoiceTotalsQueryHandler(
    SalesContext context,
    IMediator mediator,
    IAccountingYearFinancialParametersService financialParametersService,
    ILogger<GetInvoiceTotalsQueryHandler> logger)
    : IRequestHandler<GetInvoiceTotalsQuery, InvoiceTotalsResponse>
{
    public async Task<InvoiceTotalsResponse> Handle(GetInvoiceTotalsQuery request, CancellationToken cancellationToken)
    {
        var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);
        var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
        var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
        var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

        var invoiceQuery = context.Facture.AsQueryable();

        if (request.StartDate.HasValue)
            invoiceQuery = invoiceQuery.Where(f => f.Date >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            var endDateInclusive = request.EndDate.Value.TimeOfDay == TimeSpan.Zero
                ? request.EndDate.Value.Date.AddDays(1).AddTicks(-1)
                : request.EndDate.Value;
            invoiceQuery = invoiceQuery.Where(f => f.Date <= endDateInclusive);
        }

        if (request.CustomerId.HasValue)
            invoiceQuery = invoiceQuery.Where(f => f.IdClient == request.CustomerId.Value);

        if (request.Status.HasValue)
            invoiceQuery = invoiceQuery.Where(f => (int)f.Statut == request.Status.Value);

        if (request.TagIds != null && request.TagIds.Any())
        {
            var factureNumsWithTags = await context.DocumentTag
                .Where(dt => dt.DocumentType == DocumentTypes.Facture && request.TagIds.Contains(dt.TagId))
                .Select(dt => dt.DocumentId)
                .Distinct()
                .ToListAsync(cancellationToken);
            invoiceQuery = invoiceQuery.Where(f => factureNumsWithTags.Contains(f.Num));
        }

        var invoiceNumbers = await invoiceQuery.Select(f => f.Num).ToListAsync(cancellationToken);

        var linesQuery = from bdl in context.BonDeLivraison
                         where bdl.NumFacture.HasValue && invoiceNumbers.Contains(bdl.NumFacture.Value)
                         join line in context.LigneBl on bdl.Id equals line.BonDeLivraisonId
                         select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };
        var lines = await linesQuery.ToListAsync(cancellationToken);

        var vatLines = lines.Select(l => new VatLineItem(l.TotHt, l.TotTtc, l.Tva));
        var result = VatTotalsAggregator.Aggregate(vatLines, vatRate7, vatRate13, vatRate19, timbre, invoiceNumbers.Count);

        return new InvoiceTotalsResponse
        {
            TotalHT = result.TotalHT,
            TotalBase7 = result.TotalBase7,
            TotalBase13 = result.TotalBase13,
            TotalBase19 = result.TotalBase19,
            TotalVat7 = result.TotalVat7,
            TotalVat13 = result.TotalVat13,
            TotalVat19 = result.TotalVat19,
            TotalVat = result.TotalVat,
            TotalTTC = result.TotalTTC
        };
    }
}
