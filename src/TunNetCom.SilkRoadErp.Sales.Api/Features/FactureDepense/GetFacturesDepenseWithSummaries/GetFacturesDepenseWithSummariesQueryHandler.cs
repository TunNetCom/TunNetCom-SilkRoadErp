using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseWithSummaries;

public class GetFacturesDepenseWithSummariesQueryHandler(SalesContext _context, ILogger<GetFacturesDepenseWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetFacturesDepenseWithSummariesQuery, GetFacturesDepenseWithSummariesResponse>
{
    public async Task<GetFacturesDepenseWithSummariesResponse> Handle(GetFacturesDepenseWithSummariesQuery query, CancellationToken cancellationToken)
    {
        var q = _context.FactureDepense
            .AsNoTracking()
            .Include(f => f.IdTiersDepenseFonctionnementNavigation)
            .AsQueryable();

        if (query.TiersDepenseFonctionnementId.HasValue)
            q = q.Where(f => f.IdTiersDepenseFonctionnement == query.TiersDepenseFonctionnementId.Value);

        if (query.AccountingYearId.HasValue)
            q = q.Where(f => f.AccountingYearId == query.AccountingYearId.Value);

        if (query.StartDate.HasValue)
            q = q.Where(f => f.Date >= query.StartDate.Value);

        if (query.EndDate.HasValue)
        {
            var endDateInclusive = query.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(f => f.Date <= endDateInclusive);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchKeyword))
        {
            var kw = query.SearchKeyword.Trim();
            q = q.Where(f =>
                f.Description.Contains(kw) ||
                (f.IdTiersDepenseFonctionnementNavigation != null && f.IdTiersDepenseFonctionnementNavigation.Nom.Contains(kw)));
        }

        var items = await q
            .OrderByDescending(f => f.Date)
            .ThenByDescending(f => f.Num)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(f => new FactureDepenseSummaryItem
            {
                Id = f.Id,
                Num = f.Num,
                IdTiersDepenseFonctionnement = f.IdTiersDepenseFonctionnement,
                TiersDepenseFonctionnementNom = f.IdTiersDepenseFonctionnementNavigation.Nom,
                Date = f.Date,
                Description = f.Description,
                MontantTotal = f.MontantTotal,
                Statut = f.Statut.ToString(),
                HasDocument = !string.IsNullOrEmpty(f.DocumentStoragePath),
                ExonereRetenueSource = f.IdTiersDepenseFonctionnementNavigation.ExonereRetenueSource
            })
            .ToListAsync(cancellationToken);

        var totalCount = await q.CountAsync(cancellationToken);

        _logger.LogInformation("GetFacturesDepenseWithSummaries returned {Count} items", items.Count);
        return new GetFacturesDepenseWithSummariesResponse { Items = items, TotalCount = totalCount };
    }
}
