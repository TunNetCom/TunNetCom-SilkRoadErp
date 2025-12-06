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

        // Build base query with filters to avoid loading all data
        var baseQuery = from a in _context.AvoirFournisseur
                        join f in _context.Fournisseur on a.FournisseurId equals f.Id into fournisseurGroup
                        from f in fournisseurGroup.DefaultIfEmpty()
                        select new { a, f };

        // Apply filters before loading
        if (request.FournisseurId.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.a.FournisseurId == request.FournisseurId.Value);
        }

        if (request.NumFactureAvoirFournisseur.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.a.NumFactureAvoirFournisseur == request.NumFactureAvoirFournisseur.Value);
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
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            baseQuery = baseQuery.Where(x => x.a.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            baseQuery = baseQuery.Where(x =>
                (x.f != null && x.f.Nom != null && x.f.Nom.Contains(request.SearchKeyword)) ||
                x.a.Num.ToString().Contains(request.SearchKeyword));
        }

        // Query 1: Get totals directly from database (OData-style aggregation)
        // Calculate totals directly from line items without loading all data
        _logger.LogInformation("Getting totals from database");
        var totalsQuery = from x in baseQuery
                          from l in x.a.LigneAvoirFournisseur
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
        // Build query with calculations but without Statut/StatutLibelle to avoid SQL conversion issues
        _logger.LogInformation("Getting paginated data from database");
        var avoirFournisseursQueryWithTotals = baseQuery
            .Select(x => new
            {
                a = x.a,
                f = x.f,
                TotalExcludingTaxAmount = x.a.LigneAvoirFournisseur.Sum(l => l.TotHt),
                TotalVATAmount = x.a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = x.a.LigneAvoirFournisseur.Sum(l => l.TotTtc)
            });

        // Apply sorting in SQL before loading (using Statut as int for sorting)
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting avoir fournisseurs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            
            // Map StatutLibelle to Statut for SQL sorting
            var sortProperty = request.SortProperty == nameof(AvoirFournisseurBaseInfo.StatutLibelle) 
                ? nameof(AvoirFournisseurBaseInfo.Statut) 
                : request.SortProperty;
            var isAscending = request.SortOrder == SortConstants.Ascending;

            avoirFournisseursQueryWithTotals = sortProperty switch
            {
                _numColumnName => isAscending 
                    ? avoirFournisseursQueryWithTotals.OrderBy(x => x.a.Num)
                    : avoirFournisseursQueryWithTotals.OrderByDescending(x => x.a.Num),
                _dateColumnName => isAscending
                    ? avoirFournisseursQueryWithTotals.OrderBy(x => x.a.Date)
                    : avoirFournisseursQueryWithTotals.OrderByDescending(x => x.a.Date),
                _totalExcludingTaxAmountColumnName => isAscending
                    ? avoirFournisseursQueryWithTotals.OrderBy(x => x.TotalExcludingTaxAmount)
                    : avoirFournisseursQueryWithTotals.OrderByDescending(x => x.TotalExcludingTaxAmount),
                nameof(AvoirFournisseurBaseInfo.Statut) => isAscending
                    ? avoirFournisseursQueryWithTotals.OrderBy(x => (int)x.a.Statut)
                    : avoirFournisseursQueryWithTotals.OrderByDescending(x => (int)x.a.Statut),
                _ => avoirFournisseursQueryWithTotals
            };
        }

        // Get total count before pagination
        var totalCount = await avoirFournisseursQueryWithTotals.CountAsync(cancellationToken);

        // Apply pagination and load only the page needed
        var avoirFournisseursData = await avoirFournisseursQueryWithTotals
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTO in memory to avoid SQL conversion issues with Statut enum (like InvoiceBaseInfosController)
        var items = avoirFournisseursData
            .Select(x => new AvoirFournisseurBaseInfo
            {
                Num = x.a.Num,
                Date = x.a.Date,
                FournisseurId = x.a.FournisseurId,
                FournisseurName = x.f != null ? x.f.Nom : null,
                NumFactureAvoirFournisseur = x.a.NumFactureAvoirFournisseur,
                TotalExcludingTaxAmount = x.TotalExcludingTaxAmount,
                TotalVATAmount = x.TotalVATAmount,
                TotalIncludingTaxAmount = x.TotalIncludingTaxAmount,
                Statut = (int)x.a.Statut,
                StatutLibelle = x.a.Statut.ToString()
            })
            .ToList();

        var pagedAvoirFournisseurs = new PagedList<AvoirFournisseurBaseInfo>(items, totalCount, request.PageNumber, request.PageSize);

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
}

