namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNote;

public class GetReceiptNoteQueryHandler(SalesContext _context, ILogger<GetReceiptNoteQueryHandler> _logger) : IRequestHandler<GetReceiptNoteQuery, PagedList<ReceiptNoteResponse>>
{
    public async Task<PagedList<ReceiptNoteResponse>> Handle(GetReceiptNoteQuery getReceiptNoteQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Fournisseur), getReceiptNoteQuery.PageNumber, getReceiptNoteQuery.PageSize);

        IQueryable<ReceiptNoteResponse> ReceiptNoteQuery = _context.BonDeReception.Select(R =>
            new ReceiptNoteResponse
            {
                Num = R.Num,
                NumBonFournisseur = R.NumBonFournisseur,
                DateLivraison = R.DateLivraison,
                IdFournisseur = R.IdFournisseur,
                Date = R.Date,
                NumFactureFournisseur = R.NumFactureFournisseur
            })
            .AsQueryable();

        if (!string.IsNullOrEmpty(getReceiptNoteQuery.SearchKeyword))
        {
            ReceiptNoteQuery = ReceiptNoteQuery.Where(
                R => R.Num.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.NumBonFournisseur.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.DateLivraison.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.IdFournisseur.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.Date.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.NumFactureFournisseur.Equals(getReceiptNoteQuery.SearchKeyword));
        }

        PagedList<ReceiptNoteResponse> pagedReceipts = await PagedList<ReceiptNoteResponse>.ToPagedListAsync(
            ReceiptNoteQuery,
            getReceiptNoteQuery.PageNumber,
            getReceiptNoteQuery.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(Fournisseur), pagedReceipts.Count);
        return pagedReceipts;
    }
}
