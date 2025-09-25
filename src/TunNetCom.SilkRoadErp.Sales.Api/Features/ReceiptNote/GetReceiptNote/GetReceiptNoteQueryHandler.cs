namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNote;

public class GetReceiptNoteQueryHandler(SalesContext _context, ILogger<GetReceiptNoteQueryHandler> _logger)
    : IRequestHandler<GetReceiptNoteQuery, PagedList<ReceiptNoteResponse>>
{
    public async Task<PagedList<ReceiptNoteResponse>> Handle(
        GetReceiptNoteQuery getReceiptNoteQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(BonDeReception), getReceiptNoteQuery.PageNumber, getReceiptNoteQuery.PageSize);

        IQueryable<ReceiptNoteResponse> ReceiptNoteQuery =
            (from bdr in _context.BonDeReception
             join f in _context.Fournisseur on bdr.IdFournisseur equals f.Id
             select new ReceiptNoteResponse
             {
                 NumBonFournisseur = bdr.NumBonFournisseur,
                 DateLivraison = bdr.DateLivraison,
                 IdFournisseur = bdr.IdFournisseur,
                 NomFournisseur = f.Nom,
                 NumFactureFournisseur = bdr.NumFactureFournisseur
             })
             .AsNoTracking()
             .AsQueryable();

        if (!string.IsNullOrEmpty(getReceiptNoteQuery.SearchKeyword))
        {
            ReceiptNoteQuery = ReceiptNoteQuery.Where(
                R => R.Num.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.NumBonFournisseur.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.DateLivraison.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.IdFournisseur.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.Date.Equals(getReceiptNoteQuery.SearchKeyword)
                || R.NumFactureFournisseur.Equals(getReceiptNoteQuery.SearchKeyword)).AsNoTracking();
        }

        PagedList<ReceiptNoteResponse> pagedReceipts = await PagedList<ReceiptNoteResponse>.ToPagedListAsync(
            ReceiptNoteQuery,
            getReceiptNoteQuery.PageNumber,
            getReceiptNoteQuery.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(BonDeReception), pagedReceipts.Items.Count);
        return pagedReceipts;
    }
}
