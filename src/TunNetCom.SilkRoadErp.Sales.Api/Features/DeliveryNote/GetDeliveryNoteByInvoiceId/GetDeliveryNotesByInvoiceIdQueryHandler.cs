namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByInvoiceId;

public class GetDeliveryNotesByInvoiceIdQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNotesByInvoiceIdQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNotesByInvoiceIdQuery, Result<List<DeliveryNoteResponse>>>
{
    public async Task<Result<List<DeliveryNoteResponse>>> Handle(GetDeliveryNotesByInvoiceIdQuery getDeliveryNoteByInvoiceIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getDeliveryNoteByInvoiceIdQuery.NumFacture);

        var deliveryNotes = await _context.BonDeLivraison
            .Where(d => d.NumFacture == getDeliveryNoteByInvoiceIdQuery.NumFacture)
            .ToListAsync(cancellationToken);

        if (deliveryNotes is null || deliveryNotes.Count == 0)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByInvoiceIdQuery.NumFacture);

            return Result.Fail("deliveryNotes_not_found");
        }

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), deliveryNotes.Count);

        return Result.Ok(deliveryNotes.Adapt<List<DeliveryNoteResponse>>());
    }
}
