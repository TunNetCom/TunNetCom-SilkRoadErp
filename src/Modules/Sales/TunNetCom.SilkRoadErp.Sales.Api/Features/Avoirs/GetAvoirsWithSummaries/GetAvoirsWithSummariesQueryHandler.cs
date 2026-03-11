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

        // Build base query with filters to avoid loading all data
        var baseQuery = from a in _context.Avoirs.FilterByActiveAccountingYear()
                        join c in _context.Client on a.ClientId equals c.Id into clientGroup
                        from c in clientGroup.DefaultIfEmpty()
                        select new { a, c };

        // Apply filters before loading
        if (request.ClientId.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.a.ClientId == request.ClientId.Value);
        }

        // Apply Status filter
        if (request.Status.HasValue)
        {
            _logger.LogInformation("Applying status filter: {status}", request.Status);
            baseQuery = baseQuery.Where(x => (int)x.a.Statut == request.Status.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            baseQuery = baseQuery.Where(x => x.a.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            var endDateInclusive = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            _logger.LogInformation("Applying end date filter (inclusive): {endDate} -> {endDateInclusive}", request.EndDate, endDateInclusive);
            baseQuery = baseQuery.Where(x => x.a.Date <= endDateInclusive);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            baseQuery = baseQuery.Where(x =>
                (x.c != null && x.c.Nom != null && x.c.Nom.Contains(request.SearchKeyword)) ||
                x.a.Num.ToString().Contains(request.SearchKeyword));
        }

        // Query 1: Get totals directly from database (OData-style aggregation)
        // Calculate totals directly from line items without loading all data
        _logger.LogInformation("Getting totals from database");
        var totalsQuery = from x in baseQuery
                          from l in x.a.LigneAvoirs
                          select new
                          {
                              TotalExcludingTaxAmount = l.TotHt,
                              TotalVATAmount = l.TotTtc - l.TotHt,
                              TotalIncludingTaxAmount = l.TotTtc
                          };

        var totalVatAmount = await totalsQuery.SumAsync(x => x.TotalVATAmount, cancellationToken);
        var totalNetAmount = await totalsQuery.SumAsync(x => x.TotalExcludingTaxAmount, cancellationToken);
        var totalIncludingTaxAmount = await totalsQuery.SumAsync(x => x.TotalIncludingTaxAmount, cancellationToken);

        // Query 2: Get paginated data with calculations
        // Build query with calculations but without Statut conversion to avoid SQL conversion issues
        _logger.LogInformation("Getting paginated data from database");
        var avoirsQueryWithTotals = baseQuery
            .Select(x => new
            {
                a = x.a,
                c = x.c,
                TotalExcludingTaxAmount = x.a.LigneAvoirs.Sum(l => l.TotHt),
                TotalVATAmount = x.a.LigneAvoirs.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = x.a.LigneAvoirs.Sum(l => l.TotTtc)
            });

        // Apply Sorting
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting avoirs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            
            var isAscending = request.SortOrder == SortConstants.Ascending;
            
            avoirsQueryWithTotals = request.SortProperty switch
            {
                _numColumnName => isAscending
                    ? avoirsQueryWithTotals.OrderBy(x => x.a.Num)
                    : avoirsQueryWithTotals.OrderByDescending(x => x.a.Num),
                _dateColumnName => isAscending
                    ? avoirsQueryWithTotals.OrderBy(x => x.a.Date)
                    : avoirsQueryWithTotals.OrderByDescending(x => x.a.Date),
                _totalExcludingTaxAmountColumnName => isAscending
                    ? avoirsQueryWithTotals.OrderBy(x => x.TotalExcludingTaxAmount)
                    : avoirsQueryWithTotals.OrderByDescending(x => x.TotalExcludingTaxAmount),
                nameof(AvoirBaseInfo.Statut) => isAscending
                    ? avoirsQueryWithTotals.OrderBy(x => (int)x.a.Statut)
                    : avoirsQueryWithTotals.OrderByDescending(x => (int)x.a.Statut),
                _ => avoirsQueryWithTotals
            };
        }

        // Get total count before pagination
        var totalCount = await avoirsQueryWithTotals.CountAsync(cancellationToken);

        // Apply pagination and load only the page needed
        var avoirsData = await avoirsQueryWithTotals
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Map to DTO in memory to avoid SQL conversion issues with Statut enum
        var items = avoirsData
            .Select(x => new AvoirBaseInfo
            {
                Num = x.a.Num,
                Date = new DateTimeOffset(x.a.Date),
                ClientId = x.a.ClientId,
                ClientName = x.c != null ? x.c.Nom : null,
                TotalExcludingTaxAmount = x.TotalExcludingTaxAmount,
                TotalVATAmount = x.TotalVATAmount,
                TotalIncludingTaxAmount = x.TotalIncludingTaxAmount,
                Statut = (int)x.a.Statut,
                StatutLibelle = x.a.Statut.ToString()
            })
            .ToList();

        var pagedAvoirs = new PagedList<AvoirBaseInfo>(items, totalCount, request.PageNumber, request.PageSize);

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

}

