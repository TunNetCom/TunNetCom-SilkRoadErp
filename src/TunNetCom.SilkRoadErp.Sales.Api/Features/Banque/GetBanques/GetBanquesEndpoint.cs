using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.GetBanques;

public class GetBanquesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/banque", HandleGetBanquesAsync)
            .WithTags(EndpointTags.Banque);
    }

    public async Task<Ok<List<BanqueResponse>>> HandleGetBanquesAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetBanquesQuery();
        var result = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(result.Value);
    }
}

