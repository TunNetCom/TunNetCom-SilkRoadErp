namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
           "/receiptnotes/{num:int}", async Task<Results<Ok<ReceiptNoteResponse>, NotFound>> (
               IMediator mediator,
               int num,
               CancellationToken cancellationToken) =>
           {
               var query = new GetReceiptNoteByIdQuery(num);

               var result = await mediator.Send(query, cancellationToken);

               if (result.IsEntityNotFound())
               {
                   return TypedResults.NotFound();
               }

               return TypedResults.Ok(result.Value);
           })
           .WithTags(EndpointTags.ReceiptNotes);
    }
}
