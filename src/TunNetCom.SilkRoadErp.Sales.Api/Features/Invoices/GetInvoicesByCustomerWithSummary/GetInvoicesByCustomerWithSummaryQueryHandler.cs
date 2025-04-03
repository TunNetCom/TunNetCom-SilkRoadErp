namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;

public class GetInvoicesByCustomerWithSummaryQueryHandler(
    SalesContext _context,
    ILogger<GetInvoicesByCustomerWithSummaryQueryHandler> _logger)
    : IRequestHandler<GetInvoicesByCustomerWithSummaryQuery, Result<GetInvoiceListWithSummary>>
{
    public class SortProperties
    {
        public const string Num = "Num";
        public const string NetAmount = "NetAmount";
        public const string GrossAmount = "GrossAmount";
    }
    public async Task<Result<GetInvoiceListWithSummary>> Handle(
        GetInvoicesByCustomerWithSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var invoicesQueryBase = _context.Facture
            .Where(f => f.IdClient == query.ClientId);

        // TODO: replace the magic number 1000 with a constant from application settings
        var invoicesQuery = invoicesQueryBase
            .Select(f => new InvoiceResponse
            {
                Num = f.Num,
                Date = f.Date,
                TotHTva = f.BonDeLivraison.Sum(d => d.TotHTva),
                TotTva = f.BonDeLivraison.Sum(d => d.TotTva),
                TotTTC = f.BonDeLivraison.Sum(d => d.NetPayer) + 1000
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
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
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
        // TODO move magic strings to constants
        return (property, order) switch
        {
            (SortProperties.Num, SortConstants.Ascending) => query.OrderBy(d => d.Num),
            (SortProperties.Num, SortConstants.Descending) => query.OrderByDescending(d => d.Num),
            (SortProperties.NetAmount, SortConstants.Ascending) => query.OrderBy(d => d.TotTTC),
            (SortProperties.NetAmount, SortConstants.Descending) => query.OrderByDescending(d => d.TotTTC),
            (SortProperties.GrossAmount, SortConstants.Ascending) => query.OrderBy(d => d.TotHTva),
            (SortProperties.GrossAmount, SortConstants.Descending) => query.OrderByDescending(d => d.TotHTva),
            _ => query
        };
    }
}