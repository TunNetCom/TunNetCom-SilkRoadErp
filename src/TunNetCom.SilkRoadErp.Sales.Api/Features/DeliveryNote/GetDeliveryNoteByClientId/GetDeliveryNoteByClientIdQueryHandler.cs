namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByClientId;

public class GetDeliveryNoteByClientIdQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteByClientIdQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteByClientIdQuery, Result<List<DeliveryNoteResponse>>>
{
    public async Task<Result<List<DeliveryNoteResponse>>> Handle(GetDeliveryNoteByClientIdQuery getDeliveryNoteByClientIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getDeliveryNoteByClientIdQuery.ClientId);

        var deliveryNotes = await _context.BonDeLivraison
            .Where(d => d.ClientId == getDeliveryNoteByClientIdQuery.ClientId)
            .ToListAsync(cancellationToken);

        if (deliveryNotes is null || deliveryNotes.Count == 0)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByClientIdQuery.ClientId);

            return Result.Fail("deliveryNotes_not_found");
        }

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), deliveryNotes.Count);

        return Result.Ok(deliveryNotes.Adapt<List<DeliveryNoteResponse>>());
    }
}
