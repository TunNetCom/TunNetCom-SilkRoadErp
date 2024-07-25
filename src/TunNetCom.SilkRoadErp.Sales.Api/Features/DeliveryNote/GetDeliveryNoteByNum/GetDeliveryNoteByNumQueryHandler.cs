using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;

public class GetDeliveryNoteByNumQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteByNumQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteByNumQuery, Result<DeliveryNoteResponse>>
{
    public async Task<Result<DeliveryNoteResponse>> Handle(GetDeliveryNoteByNumQuery getDeliveryNoteByNumQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        var deliveryNote = await _context.BonDeLivraison.FindAsync(getDeliveryNoteByNumQuery.Num, cancellationToken);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

            return Result.Fail("deliveryNote_not_found");
        }

        _logger.LogEntityFetchedById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        return deliveryNote.Adapt<DeliveryNoteResponse>();
    }
}
