using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNotes;

public class GetUninvoicedDeliveryNotesQueryHandler(
    SalesContext _context,
    ILogger<GetUninvoicedDeliveryNotesQueryHandler> _logger)
    : IRequestHandler<GetUninvoicedDeliveryNotesQuery, Result<List<DeliveryNoteResponse>>>
{
    public async Task<Result<List<DeliveryNoteResponse>>> Handle(
        GetUninvoicedDeliveryNotesQuery getUninvoicedDeliveryNoteQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting {EntityName}s with customer id {CustomerId}",
            nameof(BonDeLivraison),
            getUninvoicedDeliveryNoteQuery.CustomerId);

        var deliveryNotes = await _context.BonDeLivraison
            .Where(d => d.ClientId == getUninvoicedDeliveryNoteQuery.CustomerId && d.NumFacture == null)
            .ToListAsync(cancellationToken);

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), deliveryNotes.Count);

        return Result.Ok(deliveryNotes.Adapt<List<DeliveryNoteResponse>>());
    }
}
