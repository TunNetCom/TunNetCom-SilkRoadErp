using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseurWithSummaries;

public class GetFactureAvoirFournisseurWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetFactureAvoirFournisseurWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetFactureAvoirFournisseurWithSummariesQuery, GetFactureAvoirFournisseurWithSummariesResponse>
{
    private const string _numColumnName = nameof(FactureAvoirFournisseurBaseInfo.Num);
    private const string _dateColumnName = nameof(FactureAvoirFournisseurBaseInfo.Date);
    private const string _totalExcludingTaxAmountColumnName = nameof(FactureAvoirFournisseurBaseInfo.TotalExcludingTaxAmount);

    public async Task<GetFactureAvoirFournisseurWithSummariesResponse> Handle(
        GetFactureAvoirFournisseurWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(FactureAvoirFournisseur), request.PageNumber, request.PageSize);

        var factureAvoirFournisseursQuery = (from f in _context.FactureAvoirFournisseur
                                            join fournisseur in _context.Fournisseur on f.IdFournisseur equals fournisseur.Id
                                            select new FactureAvoirFournisseurBaseInfo
                                            {
                                                Num = f.Num,
                                                NumFactureAvoirFourSurPage = f.NumFactureAvoirFourSurPage,
                                                Date = f.Date,
                                                IdFournisseur = f.IdFournisseur,
                                                FournisseurName = fournisseur.Nom,
                                                NumFactureFournisseur = f.NumFactureFournisseur,
                                                TotalExcludingTaxAmount = f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotHt)),
                                                TotalVATAmount = f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt)),
                                                TotalIncludingTaxAmount = f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotTtc))
                                            })
                                            .AsNoTracking()
                                            .AsQueryable();

        if (request.IdFournisseur.HasValue)
        {
            factureAvoirFournisseursQuery = factureAvoirFournisseursQuery.Where(f => f.IdFournisseur == request.IdFournisseur.Value);
        }

        if (request.NumFactureFournisseur.HasValue)
        {
            factureAvoirFournisseursQuery = factureAvoirFournisseursQuery.Where(f => f.NumFactureFournisseur == request.NumFactureFournisseur.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            factureAvoirFournisseursQuery = factureAvoirFournisseursQuery.Where(f => f.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            factureAvoirFournisseursQuery = factureAvoirFournisseursQuery.Where(f => f.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            factureAvoirFournisseursQuery = factureAvoirFournisseursQuery.Where(f =>
                (f.FournisseurName != null && f.FournisseurName.Contains(request.SearchKeyword)) ||
                f.Num.ToString().Contains(request.SearchKeyword));
        }

        // Apply Sorting
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting facture avoir fournisseurs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            factureAvoirFournisseursQuery = ApplySorting(factureAvoirFournisseursQuery, request.SortProperty, request.SortOrder);
        }

        _logger.LogInformation("Getting totals");
        var totalVatAmount = await factureAvoirFournisseursQuery.SumAsync(f => f.TotalVATAmount, cancellationToken);
        var totalNetAmount = await factureAvoirFournisseursQuery.SumAsync(f => f.TotalExcludingTaxAmount, cancellationToken);
        var totalIncludingTaxAmount = await factureAvoirFournisseursQuery.SumAsync(f => f.TotalIncludingTaxAmount, cancellationToken);

        var pagedFactureAvoirFournisseurs = await PagedList<FactureAvoirFournisseurBaseInfo>.ToPagedListAsync(
            factureAvoirFournisseursQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = new GetFactureAvoirFournisseurWithSummariesResponse
        {
            FactureAvoirFournisseurs = pagedFactureAvoirFournisseurs,
            TotalNetAmount = totalNetAmount,
            TotalVatAmount = totalVatAmount,
            TotalIncludingTaxAmount = totalIncludingTaxAmount
        };

        _logger.LogEntitiesFetched(nameof(FactureAvoirFournisseur), pagedFactureAvoirFournisseurs.Items.Count);
        return response;
    }

    private IQueryable<FactureAvoirFournisseurBaseInfo> ApplySorting(
        IQueryable<FactureAvoirFournisseurBaseInfo> factureQuery,
        string sortProperty,
        string sortOrder)
    {
        return (sortProperty, sortOrder) switch
        {
            (_numColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.Num),
            (_numColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.Num),
            (_dateColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.Date),
            (_dateColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.Date),
            (_totalExcludingTaxAmountColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.TotalExcludingTaxAmount),
            (_totalExcludingTaxAmountColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.TotalExcludingTaxAmount),
            _ => factureQuery
        };
    }
}

