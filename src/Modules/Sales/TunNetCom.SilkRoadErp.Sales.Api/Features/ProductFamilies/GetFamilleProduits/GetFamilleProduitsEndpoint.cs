using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.GetFamilleProduits;

public class GetFamilleProduitsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/product-families", Handle)
            .RequireAuthorization($"Permission:{Permissions.ViewProducts}")
            .WithTags("ProductFamilies");
    }

    public async Task<IResult> Handle(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetFamilleProduitsQuery();
        var familles = await mediator.Send(query, cancellationToken);
        return Results.Ok(familles);
    }
}

