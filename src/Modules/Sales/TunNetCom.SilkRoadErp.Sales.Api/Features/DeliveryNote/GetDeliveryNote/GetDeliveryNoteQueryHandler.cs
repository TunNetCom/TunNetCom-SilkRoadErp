using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;

public class GetDeliveryNoteQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteQuery, PagedList<DeliveryNoteResponse>>
{
    public async Task<PagedList<DeliveryNoteResponse>> Handle(
        GetDeliveryNoteQuery getDeliveryNoteQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(BonDeLivraison), getDeliveryNoteQuery.PageNumber, getDeliveryNoteQuery.PageSize);

        var deliveryNoteQuery = _context.BonDeLivraison
            .Where(d => !getDeliveryNoteQuery.IsFactured.HasValue ||
                        (getDeliveryNoteQuery.IsFactured.Value ? d.NumFacture != null : d.NumFacture == null))
            .Select(t => new DeliveryNoteResponse
            {
                DeliveryNoteNumber = t.Num,
                Date = t.Date,
                TotalExcludingTax = t.TotHTva,
                TotalVat = t.TotTva,
                TotalAmount = t.NetPayer,
                CreationTime = t.TempBl,
                InvoiceNumber = t.NumFacture,
                CustomerId = t.ClientId
            })
            .AsQueryable();

        if (!string.IsNullOrEmpty(getDeliveryNoteQuery.SearchKeyword))
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(
                d => d.DeliveryNoteNumber.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                  || d.Date.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                  || d.CustomerId.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)
                  || (d.InvoiceNumber != null && d.InvoiceNumber.ToString().Contains(getDeliveryNoteQuery.SearchKeyword)));
        }

        var pagedDeliveryNote = await PagedList<DeliveryNoteResponse>.ToPagedListAsync(
            deliveryNoteQuery,
            getDeliveryNoteQuery.PageNumber,
            getDeliveryNoteQuery.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), pagedDeliveryNote.Items.Count);

        return pagedDeliveryNote;
    }
}