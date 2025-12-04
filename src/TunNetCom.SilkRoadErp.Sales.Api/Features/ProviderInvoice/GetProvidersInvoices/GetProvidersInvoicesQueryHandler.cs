namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProvidersInvoices;

public class GetProvidersInvoicesQueryHandler(
    SalesContext _context,
    ILogger<GetProvidersInvoicesQueryHandler> _logger) :
    IRequestHandler<GetProvidersInvoicesQuery, GetProviderInvoicesWithSummary>
{
    private const string _numColumName = "Num";
    private const string _netAmountColumnName = "Date";
    private const string _grossAmountColumnName = "TotalTTC";
    public async Task<GetProviderInvoicesWithSummary> Handle(
    GetProvidersInvoicesQuery query,
    CancellationToken cancellationToken)
    {

        var invoiceQuery = (from ff in _context.FactureFournisseur
                              join br in _context.BonDeReception on ff.Num equals br.NumFactureFournisseur into brGroup
                              from br in brGroup.DefaultIfEmpty()
                              join lbr in _context.LigneBonReception on br.Id equals lbr.BonDeReceptionId into lbrGroup
                              from lbr in lbrGroup.DefaultIfEmpty()
                              join retenue in _context.RetenueSourceFournisseur on ff.Num equals retenue.NumFactureFournisseur into retenueGroup
                              from retenue in retenueGroup.DefaultIfEmpty()
                              where ff.IdFournisseur == query.IdFournisseur
                              group new { lbr, retenue } by new
                              {
                                  ff.Num,
                                  ff.IdFournisseur,
                                  ff.Date,
                                  ff.NumFactureFournisseur
                              } into g
                              select new ProviderInvoiceResponse
                              {
                                  Num = g.Key.Num,
                                  ProviderId = g.Key.IdFournisseur,
                                  Date = g.Key.Date,
                                  ProviderInvoiceNumber = g.Key.NumFactureFournisseur,
                                  TotHTva = g.Sum(x => x.lbr != null ? x.lbr.TotHt : 0),
                                  TotTTC = g.Sum(x => x.lbr != null ? x.lbr.TotTtc : 0),
                                  TotTva = g.Sum(x => x.lbr != null ? x.lbr.TotTtc : 0) - g.Sum(x => x.lbr != null ? x.lbr.TotHt : 0),
                                  HasRetenueSource = g.Any(x => x.retenue != null)
                              })
        .AsNoTracking().AsQueryable();


        if (query.SortOrder != null && query.SortProperty != null)
        {
            _logger.LogInformation("sorting invoices column : {column} order : {order}", query.SortProperty, query.SortOrder);
            invoiceQuery = ApplySorting(invoiceQuery, query.SortProperty, query.SortOrder);
        }

        // Get the paged results
        var pagedResult = await PagedList<ProviderInvoiceResponse>.ToPagedListAsync(
            invoiceQuery,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var totalGrossAmount = await invoiceQuery.SumAsync(r => r.TotHTva, cancellationToken);
        var totalNetAmount = await invoiceQuery.SumAsync(r => r.TotTTC, cancellationToken);
        var totalVATAmount = await invoiceQuery.SumAsync(r => r.TotTTC - r.TotHTva, cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} receipt notes for page {PageNumber} with page size {PageSize}",
            pagedResult.TotalCount,
            query.PageNumber,
            query.PageSize);

        var result = new GetProviderInvoicesWithSummary
        {
            Invoices = pagedResult,
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
            TotalVATAmount = totalVATAmount
        };

        return result;
    }
        private IQueryable<ProviderInvoiceResponse> ApplySorting(
            IQueryable<ProviderInvoiceResponse> invoiceQuery,
            string sortProperty,
            string sortOrder)
        {
            return SortQuery(invoiceQuery, sortProperty, sortOrder);
        }

    private IQueryable<ProviderInvoiceResponse> SortQuery(
        IQueryable<ProviderInvoiceResponse> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (_numColumName, SortConstants.Ascending) => query.OrderBy(d => d.Num),
            (_numColumName, SortConstants.Descending) => query.OrderByDescending(d => d.Num),
            (_netAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.TotTTC),
            (_netAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.TotTTC),
            (_grossAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.TotHTva),
            (_grossAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.TotHTva),
            _ => query
        };
    }
}
//TODO sorting and total count and data validation and checks 
