namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithIds
{
    public class GetInvoicesWithIdsQueryHandler(
        SalesContext _context,
        ILogger<GetInvoicesWithIdsQueryHandler> _logger)
        : IRequestHandler<GetInvoicesWithIdsQuery, Result<List<InvoiceResponse>>>
    {
        public async Task<Result<List<InvoiceResponse>>> Handle(
            GetInvoicesWithIdsQuery query,
            CancellationToken cancellationToken)
        {
            var invoices = await _context.Facture
              .Where(f => query.InvoicesIds.Contains(f.Num))
              .Select(f => new InvoiceResponse
              {
                  Number = f.Num,
                  TotalIncludingTaxAmount = f.BonDeLivraison.Sum(d => d.NetPayer),
                  TotalExcludingTaxAmount = f.BonDeLivraison.Sum(d => d.TotHTva),
                  TotalVATAmount = f.BonDeLivraison.Sum(d => d.TotTva),
                  CustomerId = f.IdClient,
              })
              .ToListAsync(cancellationToken);

            if (!invoices.Any())
            {
                _logger.LogInformation("No invoices found for IDs: {InvoiceIds}", string.Join(", ", query.InvoicesIds));
                return Result.Ok(new List<InvoiceResponse>());
            }

            _logger.LogInformation("Successfully retrieved {Count} invoices.", invoices.Count);
            return Result.Ok(invoices);
        }
    }
}
