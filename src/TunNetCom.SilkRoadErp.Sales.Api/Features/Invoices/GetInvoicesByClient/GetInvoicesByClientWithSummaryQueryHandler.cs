namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByClient;

public class GetInvoicesByClientWithSummaryQueryHandler(
    SalesContext _context,
    IInvoiceCalculator _invoiceCalculator)
    : IRequestHandler<GetInvoicesByClientWithSummaryQuery, Result<GetInvoiceListWithSummary>>
{
    public async Task<Result<GetInvoiceListWithSummary>> Handle(
        GetInvoicesByClientWithSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var invoicesQuery = _context.Facture
            .Where(f => f.IdClient == query.ClientId)
            .Select(f => new InvoiceResponse
            {
                Num = f.Num,
                Date = f.Date
            });

        var pagedInvoices = await PagedList<InvoiceResponse>
            .ToPagedListAsync(invoicesQuery, query.PageNumber, query.PageSize, cancellationToken);

        foreach (var invoice in pagedInvoices)
        {
            var facture = await _context.Facture
                .Include(f => f.BonDeLivraison)
                .FirstOrDefaultAsync(f => f.Num == invoice.Num, cancellationToken);

            if (facture != null)
            {
                invoice.TotTTC = await _invoiceCalculator.CalculateTotalTTC(facture);
                invoice.TotHTva = await _invoiceCalculator.CalculateTotalHt(facture);
                invoice.TotTva = await _invoiceCalculator.CalculateTotalTTva(facture);
            }
        }

        var totalGrossAmount = pagedInvoices.Sum(d => d.TotHTva);
        var totalVATAmount = pagedInvoices.Sum(d => d.TotTva);
        var totalNetAmount = pagedInvoices.Sum(d => d.TotTTC);

        var getInvoiceListWithSummary = new GetInvoiceListWithSummary
        {
            Invoices = pagedInvoices,
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
            TotalVATAmount = totalVATAmount
        };

        return Result.Ok(getInvoiceListWithSummary);
    }
}
