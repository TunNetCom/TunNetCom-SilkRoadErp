using Microsoft.AspNetCore.Http.HttpResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients/{id:int}", async Task<Results<Ok<ClientResponse>, NotFound>> (IMediator mediator, int id) =>
        {
            var query = new GetClientByIdQuery(id);

            if(query is null)
            {
                return TypedResults.NotFound();
            }

            var result = await mediator.Send(query);

            return TypedResults.Ok(result.Value);
        });
    }
}
