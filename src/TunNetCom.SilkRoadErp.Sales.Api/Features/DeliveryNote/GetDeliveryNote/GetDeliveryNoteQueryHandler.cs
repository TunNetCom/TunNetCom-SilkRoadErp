namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;

public class GetDeliveryNoteQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteQuery, PagedList<DeliveryNoteResponse>>
{
    public async Task<PagedList<DeliveryNoteResponse>> Handle(GetDeliveryNoteQuery getDeliveryNoteQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest("DeliveryNote", getDeliveryNoteQuery.PageNumber, getDeliveryNoteQuery.PageSize);

        var deliveryNoteQuery = _context.BonDeLivraison.Select(t =>
            new DeliveryNoteResponse
            {
                Num = t.Num,
                Date = t.Date,
                TotHTva = t.TotHTva,
                TotTva = t.TotTva,
                NetPayer = t.NetPayer,
                TempBl = t.TempBl,
                NumFacture = t.NumFacture,
                ClientId = t.ClientId
            })
            .AsQueryable();

        if (!string.IsNullOrEmpty(getDeliveryNoteQuery.SearchKeyword))
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(
                c => c.Date.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                || c.NumFacture.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                || c.Num.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                || c.ClientId.ToString().Contains(getDeliveryNoteQuery.SearchKeyword));
        }

        var pagedDeliveryNote = await PagedList<DeliveryNoteResponse>.ToPagedListAsync(
            deliveryNoteQuery,
            getDeliveryNoteQuery.PageNumber,
            getDeliveryNoteQuery.PageSize,
            cancellationToken);


        _logger.LogEntitiesFetched("DeliveryNote", pagedDeliveryNote.Count);

        return pagedDeliveryNote;
    }
}
