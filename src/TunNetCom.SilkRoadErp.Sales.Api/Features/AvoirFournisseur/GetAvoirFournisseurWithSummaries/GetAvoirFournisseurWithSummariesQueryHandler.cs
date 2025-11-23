using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseurWithSummaries;

public class GetAvoirFournisseurWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirFournisseurWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetAvoirFournisseurWithSummariesQuery, GetAvoirFournisseurWithSummariesResponse>
{
    private const string _numColumnName = nameof(AvoirFournisseurBaseInfo.Num);
    private const string _dateColumnName = nameof(AvoirFournisseurBaseInfo.Date);
    private const string _totalExcludingTaxAmountColumnName = nameof(AvoirFournisseurBaseInfo.TotalExcludingTaxAmount);

    public async Task<GetAvoirFournisseurWithSummariesResponse> Handle(
        GetAvoirFournisseurWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(AvoirFournisseur), request.PageNumber, request.PageSize);

        var avoirFournisseursQuery = (from a in _context.AvoirFournisseur
                                     join f in _context.Fournisseur on a.FournisseurId equals f.Id into fournisseurGroup
                                     from f in fournisseurGroup.DefaultIfEmpty()
                                     select new AvoirFournisseurBaseInfo
                                     {
                                         Num = a.Num,
                                         Date = a.Date,
                                         FournisseurId = a.FournisseurId,
                                         FournisseurName = f != null ? f.Nom : null,
                                         NumFactureAvoirFournisseur = a.NumFactureAvoirFournisseur,
                                         TotalExcludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotHt),
                                         TotalVATAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt),
                                         TotalIncludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc)
                                     })
                                     .AsNoTracking()
                                     .AsQueryable();

        if (request.FournisseurId.HasValue)
        {
            avoirFournisseursQuery = avoirFournisseursQuery.Where(a => a.FournisseurId == request.FournisseurId.Value);
        }

        if (request.NumFactureAvoirFournisseur.HasValue)
        {
            avoirFournisseursQuery = avoirFournisseursQuery.Where(a => a.NumFactureAvoirFournisseur == request.NumFactureAvoirFournisseur.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            avoirFournisseursQuery = avoirFournisseursQuery.Where(a => a.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            avoirFournisseursQuery = avoirFournisseursQuery.Where(a => a.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            avoirFournisseursQuery = avoirFournisseursQuery.Where(a =>
                (a.FournisseurName != null && a.FournisseurName.Contains(request.SearchKeyword)) ||
                a.Num.ToString().Contains(request.SearchKeyword));
        }

        // Apply Sorting
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting avoir fournisseurs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            avoirFournisseursQuery = ApplySorting(avoirFournisseursQuery, request.SortProperty, request.SortOrder);
        }

        _logger.LogInformation("Getting totals");
        var totalVatAmount = await avoirFournisseursQuery.SumAsync(a => a.TotalVATAmount, cancellationToken);
        var totalNetAmount = await avoirFournisseursQuery.SumAsync(a => a.TotalExcludingTaxAmount, cancellationToken);
        var totalIncludingTaxAmount = await avoirFournisseursQuery.SumAsync(a => a.TotalIncludingTaxAmount, cancellationToken);

        var pagedAvoirFournisseurs = await PagedList<AvoirFournisseurBaseInfo>.ToPagedListAsync(
            avoirFournisseursQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = new GetAvoirFournisseurWithSummariesResponse
        {
            AvoirFournisseurs = pagedAvoirFournisseurs,
            TotalNetAmount = totalNetAmount,
            TotalVatAmount = totalVatAmount,
            TotalIncludingTaxAmount = totalIncludingTaxAmount
        };

        _logger.LogEntitiesFetched(nameof(AvoirFournisseur), pagedAvoirFournisseurs.Items.Count);
        return response;
    }

    private IQueryable<AvoirFournisseurBaseInfo> ApplySorting(
        IQueryable<AvoirFournisseurBaseInfo> avoirQuery,
        string sortProperty,
        string sortOrder)
    {
        return (sortProperty, sortOrder) switch
        {
            (_numColumnName, SortConstants.Ascending) => avoirQuery.OrderBy(a => a.Num),
            (_numColumnName, SortConstants.Descending) => avoirQuery.OrderByDescending(a => a.Num),
            (_dateColumnName, SortConstants.Ascending) => avoirQuery.OrderBy(a => a.Date),
            (_dateColumnName, SortConstants.Descending) => avoirQuery.OrderByDescending(a => a.Date),
            (_totalExcludingTaxAmountColumnName, SortConstants.Ascending) => avoirQuery.OrderBy(a => a.TotalExcludingTaxAmount),
            (_totalExcludingTaxAmountColumnName, SortConstants.Descending) => avoirQuery.OrderByDescending(a => a.TotalExcludingTaxAmount),
            _ => avoirQuery
        };
    }
}

