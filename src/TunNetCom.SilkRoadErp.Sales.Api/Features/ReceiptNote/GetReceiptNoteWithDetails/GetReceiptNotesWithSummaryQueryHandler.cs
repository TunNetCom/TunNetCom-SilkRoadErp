namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails;

public class GetReceiptNotesWithSummaryQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNotesWithSummaryQueryHandler> _logger) :
    IRequestHandler<GetReceiptNotesWithSummaryQuery, ReceiptNotesWithSummaryResponse>
{
    private const string NumColumnName = "Num";
    private const string DateColumnName = "Date";
    private const string NetAmountColumnName = "TotTTC";

    public async Task<ReceiptNotesWithSummaryResponse> Handle(
        GetReceiptNotesWithSummaryQuery query,
        CancellationToken cancellationToken)
    {

        var receiptNotesQuery = (
            from rn in _context.BonDeReception
            join f in _context.Fournisseur on rn.IdFournisseur equals f.Id
            join lbr in _context.LigneBonReception on rn.Num equals lbr.NumBonRec
            group new { rn, lbr, f } by new
            {
                rn.Num,
                rn.NumBonFournisseur,
                rn.DateLivraison,
                rn.IdFournisseur,
                rn.NumFactureFournisseur,
                rn.Date,
                f.Nom
            } into g
            select new ReceiptNoteDetailsResponse
            {
                Num = (int)g.Key.Num,
                NomFournisseur = g.Key.Nom,
                NumBonFournisseur = g.Key.NumBonFournisseur,
                DateLivraison = g.Key.DateLivraison,
                IdFournisseur = g.Key.IdFournisseur,
                Date = g.Key.Date,
                NumFactureFournisseur = g.Key.NumFactureFournisseur,
                TotTTC = g.Sum(x => x.lbr.TotTtc),
                TotHTva = g.Sum(x => x.lbr.TotHt),
                TotTva = g.Sum(x => x.lbr.TotTtc) - g.Sum(x => x.lbr.TotHt)
            }
            ).AsNoTracking()
            .AsQueryable();

        if (query.IdFournisseur.HasValue)
        {
            receiptNotesQuery = receiptNotesQuery
                .Where(d => d.IdFournisseur == query.IdFournisseur);
        }
        if (query.IsInvoiced.Equals(true) && query.InvoiceId.HasValue)
        {
            receiptNotesQuery = receiptNotesQuery
                .Where(d => d.NumFactureFournisseur == query.InvoiceId);
        }
        if (query.IsInvoiced.Equals(false))
        {
            receiptNotesQuery = receiptNotesQuery
                .Where(d => !d.NumFactureFournisseur.HasValue);
        }

        if (query.queryStringParameters.StartDate.HasValue)
        {
            receiptNotesQuery = receiptNotesQuery.Where(d => d.Date >= query.queryStringParameters.StartDate.Value);
        }
        if (query.queryStringParameters.EndDate.HasValue)
        {
            receiptNotesQuery = receiptNotesQuery.Where(d => d.Date <= query.queryStringParameters.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(query.queryStringParameters.SortProprety) &&
            !string.IsNullOrEmpty(query.queryStringParameters.SortOrder))
        {
            _logger.LogInformation(
                "Sorting receipt notes by column: {Column}, order: {Order}",
                query.queryStringParameters.SortProprety,
                query.queryStringParameters.SortOrder);
            receiptNotesQuery = ApplySorting(
                receiptNotesQuery,
                query.queryStringParameters.SortProprety,
                query.queryStringParameters.SortOrder);
        }



        var pagedResult = await PagedList<ReceiptNoteDetailsResponse>.ToPagedListAsync(
            receiptNotesQuery,
            query.queryStringParameters.PageNumber,
            query.queryStringParameters.PageSize,
            cancellationToken);

        var totals = await receiptNotesQuery
            .GroupBy(_ => 1)
            .Select(g => new {
                GrossAmount = g.Sum(x => x.TotHTva),
                NetAmount = g.Sum(x => x.TotTTC)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var totalGrossAmount = totals?.GrossAmount ?? 0;
        var totalNetAmount = totals?.NetAmount ?? 0;
        var totalVATAmount = totalNetAmount - totalGrossAmount;

        _logger.LogInformation(
            "Retrieved {Count} receipt notes for page {PageNumber} with page size {PageSize}",
            pagedResult.TotalCount,
            query.queryStringParameters.PageNumber,
            query.queryStringParameters.PageSize);

        return new ReceiptNotesWithSummaryResponse
        {
            ReceiptNotes = pagedResult,
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
            TotalVATAmount = totalVATAmount
        };
    }

    private IQueryable<ReceiptNoteDetailsResponse> ApplySorting(
IQueryable<ReceiptNoteDetailsResponse> invoiceQuery,
string sortProperty,
string sortOrder)
    {
        return SortQuery(invoiceQuery, sortProperty, sortOrder);
    }

    private IQueryable<ReceiptNoteDetailsResponse> SortQuery(
        IQueryable<ReceiptNoteDetailsResponse> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (NumColumnName, SortConstants.Ascending) => query.OrderBy(d => d.Num),
            (NumColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.Num),
            (NetAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.TotTTC),
            (NetAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.TotTTC),
            (DateColumnName,SortConstants.Ascending) => query.OrderBy(d => d.Date),
            (DateColumnName,SortConstants.Descending) => query.OrderByDescending(d => d.Date),
            _ => query
        };
    }
}