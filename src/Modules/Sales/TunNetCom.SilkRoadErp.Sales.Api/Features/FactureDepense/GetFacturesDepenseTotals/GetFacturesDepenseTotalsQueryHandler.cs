using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseTotals;

public class GetFacturesDepenseTotalsQueryHandler(SalesContext context)
    : IRequestHandler<GetFacturesDepenseTotalsQuery, FactureDepenseTotalsResponse>
{
    public async Task<FactureDepenseTotalsResponse> Handle(GetFacturesDepenseTotalsQuery request, CancellationToken cancellationToken)
    {
        var q = context.FactureDepense.AsNoTracking().AsQueryable();

        if (request.AccountingYearId.HasValue)
            q = q.Where(f => f.AccountingYearId == request.AccountingYearId.Value);

        if (request.TiersDepenseFonctionnementId.HasValue)
            q = q.Where(f => f.IdTiersDepenseFonctionnement == request.TiersDepenseFonctionnementId.Value);

        if (request.StartDate.HasValue)
            q = q.Where(f => f.Date >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            var endDateInclusive = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(f => f.Date <= endDateInclusive);
        }

        var totals = await q
            .Select(f => new
            {
                HT = f.BaseHT0 + f.BaseHT7 + f.BaseHT13 + f.BaseHT19,
                TVA = f.MontantTVA0 + f.MontantTVA7 + f.MontantTVA13 + f.MontantTVA19,
                TTC = f.MontantTotal
            })
            .ToListAsync(cancellationToken);

        return new FactureDepenseTotalsResponse
        {
            TotalHT = totals.Sum(t => t.HT),
            TotalTVA = totals.Sum(t => t.TVA),
            TotalTTC = totals.Sum(t => t.TTC)
        };
    }
}
