namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;

public class GetDeliveryNotesBasedOnProductReferenceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/deliveryNoteHistory/{productReference}", async (
                IMediator mediator,
                string productReference,
                CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(productReference))
            {
                return Results.BadRequest("Product reference cannot be null or empty.");
            }

            var query = new GetDeliveryNotesBasedOnProductReferenceQuery(productReference.Trim());
            var deliveryNote = await mediator.Send(query, cancellationToken);

            return Results.Ok(deliveryNote);
        })
            .WithName("GetDeliveryNotesByProductReference")
            .WithTags(SwaggerTags.DeliveryNotes)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}