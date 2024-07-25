using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;

public class GetDeliveryNoteQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteQuery, PagedList<DeliveryNoteResponse>>
{
    public async Task<PagedList<DeliveryNoteResponse>> Handle(GetDeliveryNoteQuery getDeliveryNoteQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(BonDeLivraison), getDeliveryNoteQuery.PageNumber, getDeliveryNoteQuery.PageSize);

        var deliveryNoteQuery = _context.BonDeLivraison
            .Where(d => getDeliveryNoteQuery.IsFactured.HasValue || d.NumFacture.HasValue == getDeliveryNoteQuery.IsFactured)
            .Select(t =>
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
                d => d.Num.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                || d.Date.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                || d.ClientId.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                || d.NumFacture.ToString().Contains(getDeliveryNoteQuery.SearchKeyword));
        }

        var pagedDeliveryNote = await PagedList<DeliveryNoteResponse>.ToPagedListAsync(
            deliveryNoteQuery,
            getDeliveryNoteQuery.PageNumber,
            getDeliveryNoteQuery.PageSize,
            cancellationToken);


        _logger.LogEntitiesFetched(nameof(BonDeLivraison), pagedDeliveryNote.Count);

        return pagedDeliveryNote;
    }
}
