namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomer;

public class GetInvoicesByCustomerWithSummaryQueryHandler(
    SalesContext _context)
    : IRequestHandler<GetInvoicesByCustomerWithSummaryQuery, Result<GetInvoiceListWithSummary>>
{
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
            });

        var pagedInvoices = await PagedList<InvoiceResponse>
            .ToPagedListAsync(invoicesQuery, query.PageNumber, query.PageSize, cancellationToken);

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
}
