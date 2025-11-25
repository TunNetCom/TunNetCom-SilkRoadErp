namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProviderInvoicesWithIdsList
{
    public class GetProviderInvoicesWithIdsListQueryHandler
        : IRequestHandler<GetProviderInvoicesWithIdsListQuery, List<ProviderInvoiceResponse>>
    {
        private readonly SalesContext _context;
        private readonly ILogger<GetProviderInvoicesWithIdsListQueryHandler> _logger;

        public GetProviderInvoicesWithIdsListQueryHandler(
            SalesContext context,
            ILogger<GetProviderInvoicesWithIdsListQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProviderInvoiceResponse>> Handle(
            GetProviderInvoicesWithIdsListQuery request,
            CancellationToken cancellationToken)
        {
            // Calculate totals from receipt notes instead of using ProviderInvoiceView
            var invoices = await (from ff in _context.FactureFournisseur
                                  join br in _context.BonDeReception on ff.Num equals br.NumFactureFournisseur into receiptNotesGroup
                                  from br in receiptNotesGroup.DefaultIfEmpty()
                                  where request.InvoicesIds.Contains(ff.Num)
                                  group new { ff, br } by new
                                  {
                                      ff.Num,
                                      ff.IdFournisseur,
                                      ff.Date
                                  } into g
                                  select new ProviderInvoiceResponse
                                  {
                                      Num = g.Key.Num,
                                      ProviderId = g.Key.IdFournisseur,
                                      Date = g.Key.Date,
                                      TotHTva = g.Where(x => x.br != null).Sum(x => x.br!.TotHTva),
                                      TotTva = g.Where(x => x.br != null).Sum(x => x.br!.TotTva),
                                      TotTTC = g.Where(x => x.br != null).Sum(x => x.br!.NetPayer)
                                  })
                                  .AsNoTracking()
                                  .ToListAsync(cancellationToken);

            if (!invoices.Any())
            {
                _logger.LogInformation("No invoices found for the given IDs: {Ids}", string.Join(", ", request.InvoicesIds));
            }

            return invoices;
        }
    }
}
