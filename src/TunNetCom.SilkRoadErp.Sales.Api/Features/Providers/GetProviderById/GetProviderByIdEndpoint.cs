namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProviderById;

public class GetProviderByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/providers/{id:int}", async Task<Results<Ok<ProviderResponse>, NotFound>> (IMediator mediator, int id) =>
        {
            var query = new GetProviderByIdQuery(id);

            if (query is null)
            {
                return TypedResults.NotFound();
            }

            var result = await mediator.Send(query);

            return TypedResults.Ok(result.Value);
        });
    }
}


