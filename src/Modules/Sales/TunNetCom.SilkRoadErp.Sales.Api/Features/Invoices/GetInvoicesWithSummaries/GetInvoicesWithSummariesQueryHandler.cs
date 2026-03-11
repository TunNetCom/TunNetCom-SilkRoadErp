using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithSummaries;

public class GetInvoicesWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetInvoicesWithSummariesQueryHandler> _logger,
    IMediator mediator,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetInvoicesWithSummariesQuery, GetInvoicesWithSummariesResponse>
{
    private const string _numberColumnName = nameof(InvoiceBaseInfo.Number);
    private const string _dateColumnName = nameof(InvoiceBaseInfo.Date);
    private const string _netAmountColumnName = nameof(InvoiceBaseInfo.NetAmount);

    public async Task<GetInvoicesWithSummariesResponse> Handle(
        GetInvoicesWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        // Get timbre from financial parameters service before building the query
        var appParams = await mediator.Send(new GetAppParametersQuery());
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        _logger.LogPaginationRequest(nameof(Facture), request.PageNumber, request.PageSize);
        
        var invoicesQuery = (from f in _context.Facture
                            join c in _context.Client on f.IdClient equals c.Id
                            select new InvoiceBaseInfo
                            {
                                Number = f.Num,
                                Date = f.Date,
                                CustomerId = f.IdClient,
                                CustomerName = c.Nom,
                                NetAmount = f.BonDeLivraison.Sum(d => d.NetPayer) + timbre,
                                VatAmount = f.BonDeLivraison.Sum(d => d.TotTva)
                            })
                            .AsNoTracking()
                            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            invoicesQuery = invoicesQuery.Where(i => i.CustomerId == request.CustomerId.Value);
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            invoicesQuery = invoicesQuery.Where(i => i.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            invoicesQuery = invoicesQuery.Where(i => i.Date <= request.EndDate.Value);
        }

        // Apply search keyword
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            invoicesQuery = invoicesQuery.Where(i => 
                i.CustomerName != null && i.CustomerName.Contains(request.SearchKeyword) ||
                i.Number.ToString().Contains(request.SearchKeyword));
        }

        // Apply Sorting
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting invoices column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            invoicesQuery = ApplySorting(invoicesQuery, request.SortProperty, request.SortOrder);
        }

        _logger.LogInformation("Getting Vat and Net amounts");
        var totalVatAmount = await invoicesQuery.SumAsync(i => i.VatAmount, cancellationToken);
        var totalNetAmount = await invoicesQuery.SumAsync(i => i.NetAmount, cancellationToken);

        var pagedInvoices = await PagedList<InvoiceBaseInfo>.ToPagedListAsync(
            invoicesQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = new GetInvoicesWithSummariesResponse
        {
            Invoices = pagedInvoices,
            TotalNetAmount = totalNetAmount,
            TotalVatAmount = totalVatAmount
        };

        _logger.LogEntitiesFetched(nameof(Facture), pagedInvoices.Items.Count);
        return response;
    }

    private IQueryable<InvoiceBaseInfo> ApplySorting(
        IQueryable<InvoiceBaseInfo> invoiceQuery,
        string sortProperty,
        string sortOrder)
    {
        return SortQuery(invoiceQuery, sortProperty, sortOrder);
    }

    private IQueryable<InvoiceBaseInfo> SortQuery(
        IQueryable<InvoiceBaseInfo> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (_numberColumnName, SortConstants.Ascending) => query.OrderBy(i => i.Number),
            (_numberColumnName, SortConstants.Descending) => query.OrderByDescending(i => i.Number),
            (_dateColumnName, SortConstants.Ascending) => query.OrderBy(i => i.Date),
            (_dateColumnName, SortConstants.Descending) => query.OrderByDescending(i => i.Date),
            (_netAmountColumnName, SortConstants.Ascending) => query.OrderBy(i => i.NetAmount),
            (_netAmountColumnName, SortConstants.Descending) => query.OrderByDescending(i => i.NetAmount),
            _ => query
        };
    }
}

