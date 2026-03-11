using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.GetProviderInvoiceTotals;

public class GetProviderInvoiceTotalsQueryHandler(
    SalesContext context,
    IMediator mediator,
    IAccountingYearFinancialParametersService financialParametersService,
    ILogger<GetProviderInvoiceTotalsQueryHandler> logger)
    : IRequestHandler<GetProviderInvoiceTotalsQuery, ProviderInvoiceTotalsResponse>
{
    public async Task<ProviderInvoiceTotalsResponse> Handle(GetProviderInvoiceTotalsQuery request, CancellationToken cancellationToken)
    {
        var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
        var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
        var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

        var invoiceQuery = context.FactureFournisseur.AsQueryable();

        if (request.StartDate.HasValue)
            invoiceQuery = invoiceQuery.Where(ff => ff.Date >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            var endDateInclusive = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            invoiceQuery = invoiceQuery.Where(ff => ff.Date <= endDateInclusive);
        }

        if (request.ProviderId.HasValue)
            invoiceQuery = invoiceQuery.Where(ff => ff.IdFournisseur == request.ProviderId.Value);

        if (request.Status.HasValue)
            invoiceQuery = invoiceQuery.Where(ff => (int)ff.Statut == request.Status.Value);

        if (request.TagIds != null && request.TagIds.Any())
        {
            invoiceQuery = invoiceQuery.Where(ff => context.DocumentTag
                .Any(dt => dt.DocumentType == "FactureFournisseur"
                    && dt.DocumentId == ff.Num
                    && request.TagIds.Contains(dt.TagId)));
        }

        var invoiceNumbers = await invoiceQuery.Select(ff => ff.Num).ToListAsync(cancellationToken);

        var linesQuery = from br in context.BonDeReception
                         where br.NumFactureFournisseur.HasValue && invoiceNumbers.Contains(br.NumFactureFournisseur.Value)
                         join line in context.LigneBonReception on br.Id equals line.BonDeReceptionId
                         select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };
        var lines = await linesQuery.ToListAsync(cancellationToken);

        var vatLines = lines.Select(l => new VatLineItem(l.TotHt, l.TotTtc, l.Tva));
        var result = VatTotalsAggregator.Aggregate(vatLines, vatRate7, vatRate13, vatRate19, timbre: 0, documentCount: 0);

        return new ProviderInvoiceTotalsResponse
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
