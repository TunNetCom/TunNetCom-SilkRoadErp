using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNote;

public class GetUninvoicedDeliveryNoteQueryHandler(
    SalesContext _context,
    ILogger<GetUninvoicedDeliveryNoteQueryHandler> _logger)
    : IRequestHandler<GetUninvoicedDeliveryNoteQuery, Result<List<DeliveryNoteResponse>>>
{
    public async Task<Result<List<DeliveryNoteResponse>>> Handle(GetUninvoicedDeliveryNoteQuery getUninvoicedDeliveryNoteQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getUninvoicedDeliveryNoteQuery.ClientId);

        var deliveryNotes = await _context.BonDeLivraison
            .Where(d => d.ClientId == getUninvoicedDeliveryNoteQuery.ClientId && d.NumFacture == null)
            .ToListAsync(cancellationToken);

        if (deliveryNotes is null || deliveryNotes.Count == 0)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getUninvoicedDeliveryNoteQuery.ClientId);

            return Result.Fail("deliveryNotes_not_found");
        }

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), deliveryNotes.Count);

        return Result.Ok(deliveryNotes.Adapt<List<DeliveryNoteResponse>>());
    }
}
