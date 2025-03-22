﻿namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId;

public class GetDeliveryNotesByClientIdQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNotesByClientIdQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteByClientIdQuery, Result<List<DeliveryNoteResponse>>>
{
    public async Task<Result<List<DeliveryNoteResponse>>> Handle(
        GetDeliveryNoteByClientIdQuery getDeliveryNoteByClientIdQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getDeliveryNoteByClientIdQuery.ClientId);

        var deliveryNotes = await _context.BonDeLivraison
            .Where(d => d.ClientId == getDeliveryNoteByClientIdQuery.ClientId)
            .ToListAsync(cancellationToken);

        if (deliveryNotes is null || deliveryNotes.Count == 0)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByClientIdQuery.ClientId);

            return Result.Fail(EntityNotFound.Error("deliveryNotes_not_found"));
        }

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), deliveryNotes.Count);

        return Result.Ok(deliveryNotes.Adapt<List<DeliveryNoteResponse>>());
    }
}
