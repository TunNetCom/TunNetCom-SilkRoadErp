using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByInvoiceId;

public class GetDeliveryNotesByInvoiceIdQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNotesByInvoiceIdQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNotesByInvoiceIdQuery, Result<List<DeliveryNoteResponse>>>
{
    public async Task<Result<List<DeliveryNoteResponse>>> Handle(
        GetDeliveryNotesByInvoiceIdQuery getDeliveryNoteByInvoiceIdQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retreiving {EntityName} with invoice id : {InvoiceId}",
            nameof(BonDeLivraison),
            getDeliveryNoteByInvoiceIdQuery.NumFacture);

        List<BonDeLivraison> deliveryNotes = await _context.BonDeLivraison
            .Where(d => d.NumFacture == getDeliveryNoteByInvoiceIdQuery.NumFacture)
            .ToListAsync(cancellationToken);

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), deliveryNotes.Count);

        return Result.Ok(deliveryNotes.Adapt<List<DeliveryNoteResponse>>());
    }
}