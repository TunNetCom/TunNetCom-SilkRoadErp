using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByInvoiceId;

public class GetDeliveryNotesByInvoiceIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/deliveryNote/facture/{NumFacture:int}",
            async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> 
            (
                IMediator mediator,
                int NumFacture,
                CancellationToken cancellationToken) =>
            {
                var query = new GetDeliveryNotesByInvoiceIdQuery(NumFacture);

                Result<List<DeliveryNoteResponse>> result = await mediator.Send(query, cancellationToken);

                if (result.IsEntityNotFound())
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(result.Value);
            });
    }
}
