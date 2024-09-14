using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
           "/receiptnotes/{num:int}", async Task<Results<Ok<ReceiptNoteResponse>, NotFound>> (
               IMediator mediator,
               int num,
               CancellationToken cancellationToken) =>
           {
               var query = new GetReceiptNoteByIdQuery(num);

               var result = await mediator.Send(query, cancellationToken);

               if (result.IsFailed)
               {
                   return TypedResults.NotFound();
               }

               return TypedResults.Ok(result.Value);
           });
    }
}
