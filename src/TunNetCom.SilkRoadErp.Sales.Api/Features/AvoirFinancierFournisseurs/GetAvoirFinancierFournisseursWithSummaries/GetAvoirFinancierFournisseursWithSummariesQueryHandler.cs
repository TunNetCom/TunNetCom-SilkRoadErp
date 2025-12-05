using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseursWithSummaries;

public class GetAvoirFinancierFournisseursWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirFinancierFournisseursWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetAvoirFinancierFournisseursWithSummariesQuery, GetAvoirFinancierFournisseursWithSummariesResponse>
{
    private const string _numColumnName = nameof(AvoirFinancierFournisseursBaseInfo.Num);
    private const string _dateColumnName = nameof(AvoirFinancierFournisseursBaseInfo.Date);
    private const string _totTtcColumnName = nameof(AvoirFinancierFournisseursBaseInfo.TotTtc);

    public async Task<GetAvoirFinancierFournisseursWithSummariesResponse> Handle(
        GetAvoirFinancierFournisseursWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Domain.Entites.AvoirFinancierFournisseurs), request.PageNumber, request.PageSize);

        var avoirFinancierQuery = (from a in _context.AvoirFinancierFournisseurs
                                    join ff in _context.FactureFournisseur on a.Num equals ff.Num
                                    join fournisseur in _context.Fournisseur on ff.IdFournisseur equals fournisseur.Id
                                    select new AvoirFinancierFournisseursBaseInfo
                                    {
                                        Num = a.Num,
                                        NumSurPage = a.NumSurPage,
                                        Date = a.Date,
                                        Description = a.Description,
                                        TotTtc = a.TotTtc,
                                        NumFactureFournisseur = a.Num,
                                        ProviderId = ff.IdFournisseur,
                                        ProviderName = fournisseur.Nom,
                                        ProviderInvoiceNumber = ff.NumFactureFournisseur
                                    })
                                    .AsNoTracking()
                                    .AsQueryable();

        if (request.ProviderId.HasValue)
        {
            avoirFinancierQuery = avoirFinancierQuery.Where(a => a.ProviderId == request.ProviderId.Value);
        }

        if (request.NumFactureFournisseur.HasValue)
        {
            avoirFinancierQuery = avoirFinancierQuery.Where(a => a.NumFactureFournisseur == request.NumFactureFournisseur.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            avoirFinancierQuery = avoirFinancierQuery.Where(a => a.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            avoirFinancierQuery = avoirFinancierQuery.Where(a => a.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            avoirFinancierQuery = avoirFinancierQuery.Where(a =>
                (a.ProviderName != null && a.ProviderName.Contains(request.SearchKeyword)) ||
                a.Num.ToString().Contains(request.SearchKeyword) ||
                a.ProviderInvoiceNumber.ToString().Contains(request.SearchKeyword));
        }

        // Apply Sorting
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting avoir financier fournisseurs column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            avoirFinancierQuery = ApplySorting(avoirFinancierQuery, request.SortProperty, request.SortOrder);
        }

        _logger.LogInformation("Getting totals");
        var totalAmount = await avoirFinancierQuery.SumAsync(a => a.TotTtc, cancellationToken);

        var pagedAvoirFinancier = await PagedList<AvoirFinancierFournisseursBaseInfo>.ToPagedListAsync(
            avoirFinancierQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = new GetAvoirFinancierFournisseursWithSummariesResponse
        {
            AvoirFinancierFournisseurs = pagedAvoirFinancier,
            TotalAmount = totalAmount
        };

        _logger.LogEntitiesFetched(nameof(Domain.Entites.AvoirFinancierFournisseurs), pagedAvoirFinancier.Items.Count);
        return response;
    }

    private IQueryable<AvoirFinancierFournisseursBaseInfo> ApplySorting(
        IQueryable<AvoirFinancierFournisseursBaseInfo> query,
        string sortProperty,
        string sortOrder)
    {
        return (sortProperty, sortOrder) switch
        {
            (_numColumnName, SortConstants.Ascending) => query.OrderBy(a => a.Num),
            (_numColumnName, SortConstants.Descending) => query.OrderByDescending(a => a.Num),
            (_dateColumnName, SortConstants.Ascending) => query.OrderBy(a => a.Date),
            (_dateColumnName, SortConstants.Descending) => query.OrderByDescending(a => a.Date),
            (_totTtcColumnName, SortConstants.Ascending) => query.OrderBy(a => a.TotTtc),
            (_totTtcColumnName, SortConstants.Descending) => query.OrderByDescending(a => a.TotTtc),
            _ => query
        };
    }
}

