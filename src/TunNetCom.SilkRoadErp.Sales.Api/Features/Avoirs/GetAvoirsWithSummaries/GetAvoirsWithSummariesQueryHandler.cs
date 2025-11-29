using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoirsWithSummaries;

public class GetAvoirsWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirsWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetAvoirsWithSummariesQuery, GetAvoirsWithSummariesResponse>
{
    private const string _numColumnName = nameof(AvoirBaseInfo.Num);
    private const string _dateColumnName = nameof(AvoirBaseInfo.Date);
    private const string _totalExcludingTaxAmountColumnName = nameof(AvoirBaseInfo.TotalExcludingTaxAmount);

    public async Task<GetAvoirsWithSummariesResponse> Handle(
        GetAvoirsWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Avoirs), request.PageNumber, request.PageSize);

        var avoirsQuery = (from a in _context.Avoirs
                          join c in _context.Client on a.ClientId equals c.Id into clientGroup
                          from c in clientGroup.DefaultIfEmpty()
                          select new AvoirBaseInfo
                          {
                              Num = a.Num,
                              Date = a.Date,
                              ClientId = a.ClientId,
                              ClientName = c != null ? c.Nom : null,
                              TotalExcludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotHt),
                              TotalVATAmount = a.LigneAvoirs.Sum(l => l.TotTtc - l.TotHt),
                              TotalIncludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotTtc),
                              Statut = (int)a.Statut
                          })
                          .AsNoTracking()
                          .AsQueryable();

        if (request.ClientId.HasValue)
        {
            avoirsQuery = avoirsQuery.Where(a => a.ClientId == request.ClientId.Value);
        }

        // Apply Status filter
        if (request.Status.HasValue)
        {
            _logger.LogInformation("Applying status filter: {status}", request.Status);
            avoirsQuery = avoirsQuery.Where(a => a.Statut == request.Status.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            avoirsQuery = avoirsQuery.Where(a => a.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            avoirsQuery = avoirsQuery.Where(a => a.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            avoirsQuery = avoirsQuery.Where(a =>
                (a.ClientName != null && a.ClientName.Contains(request.SearchKeyword)) ||
                a.Num.ToString().Contains(request.SearchKeyword));
        }

        // Apply Sorting
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting avoirs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            avoirsQuery = ApplySorting(avoirsQuery, request.SortProperty, request.SortOrder);
        }

        _logger.LogInformation("Getting totals");
        var totalVatAmount = await avoirsQuery.SumAsync(a => a.TotalVATAmount, cancellationToken);
        var totalNetAmount = await avoirsQuery.SumAsync(a => a.TotalExcludingTaxAmount, cancellationToken);
        var totalIncludingTaxAmount = await avoirsQuery.SumAsync(a => a.TotalIncludingTaxAmount, cancellationToken);

        var pagedAvoirs = await PagedList<AvoirBaseInfo>.ToPagedListAsync(
            avoirsQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = new GetAvoirsWithSummariesResponse
        {
            Avoirs = pagedAvoirs,
            TotalNetAmount = totalNetAmount,
            TotalVatAmount = totalVatAmount,
            TotalIncludingTaxAmount = totalIncludingTaxAmount
        };

        _logger.LogEntitiesFetched(nameof(Avoirs), pagedAvoirs.Items.Count);
        return response;
    }

    private IQueryable<AvoirBaseInfo> ApplySorting(
        IQueryable<AvoirBaseInfo> avoirQuery,
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

