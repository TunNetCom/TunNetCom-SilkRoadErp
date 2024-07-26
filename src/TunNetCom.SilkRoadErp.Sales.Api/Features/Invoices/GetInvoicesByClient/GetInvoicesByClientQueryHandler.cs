namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByClient;

public class GetInvoicesByClientQueryHandler(
    SalesContext _context,
    IInvoiceCalculator _invoiceCalculator
     )
    : IRequestHandler<GetInvoicesByClientQuery, Result<PagedList<InvoiceResponse>>>
{
    public async Task<Result<PagedList<InvoiceResponse>>> Handle(GetInvoicesByClientQuery query, CancellationToken cancellationToken)
    {
        var invoicesQuery = _context.Facture
            .Where(f => f.IdClient == query.ClientId)
            .Select(f => new InvoiceResponse
            {
                Num = f.Num,
                Date = f.Date,
            });

        var pagedInvoices = await PagedList<InvoiceResponse>.ToPagedListAsync(invoicesQuery, query.PageNumber, query.PageSize, cancellationToken);

        foreach (var invoice in pagedInvoices)
        {
            var facture = await _context.Facture
                .Include(f => f.BonDeLivraison)
                .FirstOrDefaultAsync(f => f.Num == invoice.Num, cancellationToken);

            if (facture != null)
            {
                invoice.TotTTC = await _invoiceCalculator.CalculateTotalTTC(facture);
            }
        }

        return Result.Ok(pagedInvoices);
    }
}
