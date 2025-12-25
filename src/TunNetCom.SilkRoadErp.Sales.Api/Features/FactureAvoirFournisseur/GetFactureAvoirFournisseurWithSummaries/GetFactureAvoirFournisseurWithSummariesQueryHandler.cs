using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseurWithSummaries;

public class GetFactureAvoirFournisseurWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetFactureAvoirFournisseurWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetFactureAvoirFournisseurWithSummariesQuery, GetFactureAvoirFournisseurWithSummariesResponse>
{
    private const string _idColumnName = nameof(FactureAvoirFournisseurBaseInfo.Id);
    private const string _dateColumnName = nameof(FactureAvoirFournisseurBaseInfo.Date);
    private const string _totalExcludingTaxAmountColumnName = nameof(FactureAvoirFournisseurBaseInfo.TotalExcludingTaxAmount);
    private const string _statutColumnName = nameof(FactureAvoirFournisseurBaseInfo.Statut);
    private const string _statutLibelleColumnName = nameof(FactureAvoirFournisseurBaseInfo.StatutLibelle);

    public async Task<GetFactureAvoirFournisseurWithSummariesResponse> Handle(
        GetFactureAvoirFournisseurWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(FactureAvoirFournisseur), request.PageNumber, request.PageSize);

        // Build base query with filters to avoid loading all data
        var baseQuery = from f in _context.FactureAvoirFournisseur.FilterByActiveAccountingYear()
                        join fournisseur in _context.Fournisseur on f.IdFournisseur equals fournisseur.Id
                        join accountingYear in _context.AccountingYear on f.AccountingYearId equals accountingYear.Id
                        select new { f, fournisseur, accountingYear };

        // Apply filters before loading
        if (request.IdFournisseur.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.f.IdFournisseur == request.IdFournisseur.Value);
        }

        if (request.FactureFournisseurId.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.f.FactureFournisseurId == request.FactureFournisseurId.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            baseQuery = baseQuery.Where(x => x.f.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            baseQuery = baseQuery.Where(x => x.f.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            baseQuery = baseQuery.Where(x =>
                (x.fournisseur.Nom != null && x.fournisseur.Nom.Contains(request.SearchKeyword)) ||
                x.f.Id.ToString().Contains(request.SearchKeyword));
        }

        // Query 1: Get totals directly from database (OData-style aggregation)
        // Calculate totals directly from line items without loading all data
        _logger.LogInformation("Getting totals from database");
        var totalsQuery = from x in baseQuery
                          from a in x.f.AvoirFournisseur
                          from l in a.LigneAvoirFournisseur
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
        // Load data first to avoid SQL conversion issues with Statut (enum -> string -> int)
        _logger.LogInformation("Getting paginated data from database");
        
        // Build query with calculations but without Statut/StatutLibelle to avoid SQL conversion issues
        var factureAvoirQueryWithTotals = baseQuery
            .Select(x => new
            {
                f = x.f,
                fournisseur = x.fournisseur,
                accountingYear = x.accountingYear,
                TotalExcludingTaxAmount = x.f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotHt)),
                TotalVATAmount = x.f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt)),
                TotalIncludingTaxAmount = x.f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotTtc))
            });

        // Apply sorting in SQL before loading (using Statut as int for sorting)
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting facture avoir fournisseurs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            
            // Map StatutLibelle to Statut for SQL sorting
            var sortProperty = request.SortProperty == _statutLibelleColumnName ? _statutColumnName : request.SortProperty;
            var isAscending = request.SortOrder == SortConstants.Ascending;

            factureAvoirQueryWithTotals = sortProperty switch
            {
                _idColumnName => isAscending 
                    ? factureAvoirQueryWithTotals.OrderBy(x => x.f.Id)
                    : factureAvoirQueryWithTotals.OrderByDescending(x => x.f.Id),
                _dateColumnName => isAscending
                    ? factureAvoirQueryWithTotals.OrderBy(x => x.f.Date)
                    : factureAvoirQueryWithTotals.OrderByDescending(x => x.f.Date),
                _totalExcludingTaxAmountColumnName => isAscending
                    ? factureAvoirQueryWithTotals.OrderBy(x => x.TotalExcludingTaxAmount)
                    : factureAvoirQueryWithTotals.OrderByDescending(x => x.TotalExcludingTaxAmount),
                _statutColumnName => isAscending
                    ? factureAvoirQueryWithTotals.OrderBy(x => (int)x.f.Statut)
                    : factureAvoirQueryWithTotals.OrderByDescending(x => (int)x.f.Statut),
                _ => factureAvoirQueryWithTotals
            };
        }

        // Get total count before pagination
        var totalCount = await factureAvoirQueryWithTotals.CountAsync(cancellationToken);

        // Apply pagination and load only the page needed
        var factureAvoirData = await factureAvoirQueryWithTotals
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTO in memory to avoid SQL conversion issues with Statut enum (like InvoiceBaseInfosController)
        var items = factureAvoirData
            .Select(x => new FactureAvoirFournisseurBaseInfo
            {
                Id = x.f.Id,
                NumFactureAvoirFourSurPage = x.f.NumFactureAvoirFourSurPage,
                Date = new DateTimeOffset(x.f.Date, TimeSpan.Zero),
                IdFournisseur = x.f.IdFournisseur,
                FournisseurName = x.fournisseur?.Nom,
                NumFactureFournisseur = x.f.FactureFournisseurId,
                AccountingYearId = x.f.AccountingYearId,
                AccountingYearName = x.accountingYear != null ? x.accountingYear.Year.ToString() : string.Empty,
                TotalExcludingTaxAmount = x.TotalExcludingTaxAmount,
                TotalVATAmount = x.TotalVATAmount,
                TotalIncludingTaxAmount = x.TotalIncludingTaxAmount,
                Statut = (int)x.f.Statut,
                StatutLibelle = x.f.Statut.ToString()
            })
            .ToList();

        var pagedFactureAvoirFournisseurs = new PagedList<FactureAvoirFournisseurBaseInfo>(items, totalCount, request.PageNumber, request.PageSize);

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
            (_idColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.Id),
            (_idColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.Id),
            (_dateColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.Date),
            (_dateColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.Date),
            (_totalExcludingTaxAmountColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.TotalExcludingTaxAmount),
            (_totalExcludingTaxAmountColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.TotalExcludingTaxAmount),
            (_statutColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.Statut),
            (_statutColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.Statut),
            (_statutLibelleColumnName, SortConstants.Ascending) => factureQuery.OrderBy(f => f.Statut), // Sort by Statut (int) when sorting by StatutLibelle
            (_statutLibelleColumnName, SortConstants.Descending) => factureQuery.OrderByDescending(f => f.Statut), // Sort by Statut (int) when sorting by StatutLibelle
            _ => factureQuery
        };
    }
}

