using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetFournisseursAvecProblemesSolde;

public class GetFournisseursAvecProblemesSoldeQueryHandler(
    ILogger<GetFournisseursAvecProblemesSoldeQueryHandler> _logger,
    IActiveAccountingYearService _activeAccountingYearService,
    ISoldeFournisseurCalculationService _soldeFournisseurCalculationService)
    : IRequestHandler<GetFournisseursAvecProblemesSoldeQuery, PagedList<FournisseurSoldeProblemeResponse>>
{
    public async Task<PagedList<FournisseurSoldeProblemeResponse>> Handle(
        GetFournisseursAvecProblemesSoldeQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "GetFournisseursAvecProblemesSoldeQuery called with PageNumber={PageNumber}, PageSize={PageSize}",
            query.PageNumber, query.PageSize);

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!activeYearId.HasValue)
            {
                _logger.LogWarning("No active accounting year found");
                return new PagedList<FournisseurSoldeProblemeResponse>(
                    new List<FournisseurSoldeProblemeResponse>(), 0, query.PageNumber, query.PageSize);
            }
            accountingYearId = activeYearId.Value;
        }

        var allSoldes = await _soldeFournisseurCalculationService.GetSoldesFournisseursForAccountingYearAsync(
            accountingYearId.Value, cancellationToken);

        var withProblems = allSoldes
            .Where(x => x.Solde != 0)
            .Select(x => new FournisseurSoldeProblemeResponse
            {
                FournisseurId = x.FournisseurId,
                FournisseurNom = x.FournisseurNom,
                Solde = x.Solde,
                TotalFactures = x.TotalFactures,
                TotalFacturesAvoir = x.TotalFacturesAvoir,
                TotalAvoirsFinanciers = x.TotalAvoirsFinanciers,
                TotalPaiements = x.TotalPaiements,
                DateDernierDocument = x.DateDernierDocument
            })
            .ToList();

        var totalCount = withProblems.Count;
        var pagedItems = withProblems
            .OrderByDescending(r => r.Solde < 0 ? 1 : 0)
            .ThenByDescending(r => r.DateDernierDocument ?? DateTime.MinValue)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        _logger.LogInformation("Found {Count} fournisseurs with solde problems", totalCount);

        return new PagedList<FournisseurSoldeProblemeResponse>(
            pagedItems, totalCount, query.PageNumber, query.PageSize);
    }
}
