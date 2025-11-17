namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProviderById;

public class GetProviderByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/providers/{id:int}",
            async Task<Results<Ok<ProviderResponse>, NotFound>> (
                IMediator mediator,
                int id) =>
        {
            var query = new GetProviderByIdQuery(id);

            var result = await mediator.Send(query);

            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value);
        })
        .WithTags(SwaggerTags.Providers);
    }
}


