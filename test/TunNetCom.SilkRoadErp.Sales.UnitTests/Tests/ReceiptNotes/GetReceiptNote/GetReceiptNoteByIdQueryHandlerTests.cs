using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNote;
public class GetReceiptNoteQueryHandler(SalesContext _context, ILogger<GetReceiptNoteQueryHandler> _logger)
    : IRequestHandler<GetReceiptNoteQuery, PagedList<ReceiptNoteResponse>>
{
    public async Task<PagedList<ReceiptNoteResponse>> Handle(
        GetReceiptNoteQuery getReceiptNoteQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(BonDeReception), getReceiptNoteQuery.PageNumber, getReceiptNoteQuery.PageSize);
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
            string keyword = getReceiptNoteQuery.SearchKeyword;

            ReceiptNoteQuery = ReceiptNoteQuery.Where(R =>
                R.Num.ToString() == keyword
                || R.NumBonFournisseur.ToString() == keyword
                || R.DateLivraison.ToString() == keyword
                || R.IdFournisseur.ToString() == keyword
                || R.Date.ToString() == keyword
                || (R.NumFactureFournisseur.HasValue && R.NumFactureFournisseur.Value.ToString() == keyword)
            );
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
