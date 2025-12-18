using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;

public class GetInvoicesByCustomerWithSummaryQueryHandler(
    SalesContext _context,
    ILogger<GetInvoicesByCustomerWithSummaryQueryHandler> _logger,
    IMediator mediator)
    : IRequestHandler<GetInvoicesByCustomerWithSummaryQuery, Result<GetInvoiceListWithSummary>>
{
    private const string _numberColumnName = nameof(InvoiceResponse.Number);
    private const string _dateColumnName = nameof(InvoiceResponse.Date);
    private const string _grossAmountColumnName = nameof(InvoiceResponse.TotalExcludingTaxAmount);
    private const string _netAmountColumnName = nameof(InvoiceResponse.TotalIncludingTaxAmount);

    public async Task<Result<GetInvoiceListWithSummary>> Handle(
        GetInvoicesByCustomerWithSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var appParams = await mediator.Send(new GetAppParametersQuery());

        _logger.LogPaginationRequest(nameof(Facture), query.PageNumber, query.PageSize);
        var invoicesQueryBase = _context.Facture
            .Where(f => f.IdClient == query.ClientId);

        var invoicesQuery = invoicesQueryBase
            .Select(f => new InvoiceResponse
            {
                Id = f.Id,
                Number = f.Num,
                Date = f.Date,
                TotalExcludingTaxAmount = f.BonDeLivraison.Sum(d => d.TotHTva),
                TotalVATAmount = f.BonDeLivraison.Sum(d => d.TotTva),
                TotalIncludingTaxAmount = f.BonDeLivraison.Sum(d => d.NetPayer) + appParams.Value.Timbre,
                HasRetenueSource = _context.RetenueSourceClient.Any(r => r.NumFacture == f.Num)
            })
            .AsQueryable();

        if(query.SortOrder != null && query.SortProperty != null)
        {
            _logger.LogInformation("sorting invoices column : {column} order : {order}", query.SortProperty, query.SortOrder);
            invoicesQuery = ApplySorting(invoicesQuery, query.SortProperty, query.SortOrder);
        }

        var pagedInvoices = await PagedList<InvoiceResponse>.ToPagedListAsync(
            source: invoicesQuery,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken);

        var totalGrossAmount = await invoicesQueryBase.SumAsync(d => d.BonDeLivraison.Sum(d => d.TotHTva));
        var totalVATAmount = await invoicesQueryBase.SumAsync(d => d.BonDeLivraison.Sum(d => d.TotTva));
        var totalNetAmount = await invoicesQueryBase.SumAsync(d => d.BonDeLivraison.Sum(d => d.NetPayer));

        var getInvoiceListWithSummary = new GetInvoiceListWithSummary
        {
            Invoices = pagedInvoices,
            TotalExcludingTaxAmount = totalGrossAmount,
            TotalIncludingTaxAmount = totalNetAmount,
            TotalVATAmount = totalVATAmount
        };

        return Result.Ok(getInvoiceListWithSummary);
    }

    private IQueryable<InvoiceResponse> ApplySorting(
        IQueryable<InvoiceResponse> invoiceQuery,
        string sortProperty,
        string sortOrder)
    {
        return SortQuery(invoiceQuery, sortProperty, sortOrder);
    }

    private IQueryable<InvoiceResponse> SortQuery(
        IQueryable<InvoiceResponse> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (_numberColumnName, SortConstants.Ascending) => query.OrderBy(d => d.Number),
            (_numberColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.Number),
            (_dateColumnName, SortConstants.Ascending) => query.OrderBy(d => d.Date),
            (_dateColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.Date),
            (_netAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.TotalIncludingTaxAmount),
            (_netAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.TotalIncludingTaxAmount),
            (_grossAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.TotalExcludingTaxAmount),
            (_grossAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.TotalExcludingTaxAmount),
            _ => query
        };
    }
}