using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProvidersInvoices;

public class GetProvidersInvoicesQueryHandler(
    SalesContext _context,
    ILogger<GetProvidersInvoicesQueryHandler> _logger,
    IMediator _mediator,
    IAccountingYearFinancialParametersService _financialParametersService) :
    IRequestHandler<GetProvidersInvoicesQuery, GetProviderInvoicesWithSummary>
{
    private const string _numColumName = "Num";
    private const string _netAmountColumnName = "Date";
    private const string _grossAmountColumnName = "TotalTTC";
    public async Task<GetProviderInvoicesWithSummary> Handle(
    GetProvidersInvoicesQuery query,
    CancellationToken cancellationToken)
    {
        // Get timbre from financial parameters service
        var appParamsResult = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var fallbackTimbre = appParamsResult.IsSuccess ? appParamsResult.Value.Timbre : 0;
        var timbre = await _financialParametersService.GetTimbreAsync(fallbackTimbre, cancellationToken);

        var invoiceQuery = (from ff in _context.FactureFournisseur.FilterByActiveAccountingYear()
                              join br in _context.BonDeReception on ff.Num equals br.NumFactureFournisseur into brGroup
                              from br in brGroup.DefaultIfEmpty()
                              join lbr in _context.LigneBonReception on br.Id equals lbr.BonDeReceptionId into lbrGroup
                              from lbr in lbrGroup.DefaultIfEmpty()
                              join retenue in _context.RetenueSourceFournisseur on ff.Num equals retenue.NumFactureFournisseur into retenueGroup
                              from retenue in retenueGroup.DefaultIfEmpty()
                              where ff.IdFournisseur == query.IdFournisseur
                              group new { lbr, retenue, ff } by new
                              {
                                  ff.Id,
                                  ff.Num,
                                  ff.IdFournisseur,
                                  ff.Date,
                                  ff.NumFactureFournisseur
                              } into g
                              select new ProviderInvoiceResponse
                              {
                                  Id = g.Key.Id,
                                  Num = g.Key.Num,
                                  ProviderId = g.Key.IdFournisseur,
                                  Date = g.Key.Date,
                                  ProviderInvoiceNumber = g.Key.NumFactureFournisseur,
                                  TotHTva = g.Sum(x => x.lbr != null ? x.lbr.TotHt : 0),
                                  TotTTC = g.Sum(x => x.lbr != null ? x.lbr.TotTtc : 0) + timbre, // Ajouter le timbre au TotTTC
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

        var invoiceNums = pagedResult.Items.Select(i => i.Num).ToList();
        var invoiceIds = pagedResult.Items.Select(i => i.Id).ToList();

        var avoirsByInvoice = await _context.AvoirFinancierFournisseurs
            .AsNoTracking()
            .Where(a => a.NumFactureFournisseur != null && invoiceNums.Contains(a.NumFactureFournisseur.Value))
            .Select(a => new { a.NumFactureFournisseur!.Value, Avoir = new AvoirFinancierSummary { Id = a.Id, Num = a.Num, Date = a.Date, TotTtc = a.TotTtc, Description = a.Description } })
            .ToListAsync(cancellationToken);
        var avoirsGrouped = avoirsByInvoice.GroupBy(x => x.Value).ToDictionary(g => g.Key, g => g.Select(x => x.Avoir).ToList());

        // Total et liste des factures avoir fournisseur (avoirs normaux) rattachées à chaque facture (par FactureFournisseurId = Id)
        var facturesAvoirTotals = await _context.FactureAvoirFournisseur
            .AsNoTracking()
            .Where(fa => fa.FactureFournisseurId != null && invoiceIds.Contains(fa.FactureFournisseurId.Value))
            .Select(fa => new { fa.FactureFournisseurId, Total = fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc) })
            .ToListAsync(cancellationToken);
        var facturesAvoirByInvoiceId = facturesAvoirTotals
            .GroupBy(x => x.FactureFournisseurId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Total));

        var facturesAvoirList = await _context.FactureAvoirFournisseur
            .AsNoTracking()
            .Where(fa => fa.FactureFournisseurId != null && invoiceIds.Contains(fa.FactureFournisseurId.Value))
            .Select(fa => new
            {
                fa.FactureFournisseurId,
                Summary = new FactureAvoirSummary
                {
                    Id = fa.Id,
                    NumFactureAvoirFourSurPage = fa.NumFactureAvoirFourSurPage,
                    Date = fa.Date,
                    TotTtc = fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc)
                }
            })
            .ToListAsync(cancellationToken);
        var facturesAvoirGrouped = facturesAvoirList
            .GroupBy(x => x.FactureFournisseurId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Summary).ToList());

        foreach (var invoice in pagedResult.Items)
        {
            invoice.AvoirsFinanciers = avoirsGrouped.TryGetValue(invoice.Num, out var list) ? list : new List<AvoirFinancierSummary>();
            invoice.TotalFacturesAvoir = facturesAvoirByInvoiceId.TryGetValue(invoice.Id, out var totalFa) ? totalFa : 0;
            invoice.FacturesAvoir = facturesAvoirGrouped.TryGetValue(invoice.Id, out var facturesAvoir) ? facturesAvoir : new List<FactureAvoirSummary>();
        }

        var totalGrossAmount = await invoiceQuery.SumAsync(r => r.TotHTva, cancellationToken);
        var totalNetAmount = await invoiceQuery.SumAsync(r => r.TotTTC, cancellationToken);
        var totalVATAmount = await invoiceQuery.SumAsync(r => r.TotTTC - r.TotHTva, cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} receipt notes for page {PageNumber} with page size {PageSize}",
            pagedResult.TotalCount,
            query.PageNumber,
            query.PageSize);

        // TotalNetAmount = HT (hors taxes), TotalGrossAmount = TTC (toutes taxes comprises)
        var result = new GetProviderInvoicesWithSummary
        {
            Invoices = pagedResult,
            TotalGrossAmount = totalNetAmount,
            TotalNetAmount = totalGrossAmount,
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
