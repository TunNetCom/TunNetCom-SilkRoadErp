using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.GetSousFamilleProduits;

public class GetSousFamilleProduitsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/product-subfamilies", Handle)
            .RequireAuthorization($"Permission:{Permissions.ViewProducts}")
            .WithTags("ProductSubFamilies");
    }

    public async Task<IResult> Handle(
        [AsParameters] GetSousFamilleProduitsQueryParams queryParams,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetSousFamilleProduitsQuery(queryParams.FamilleProduitId);
        var sousFamilles = await mediator.Send(query, cancellationToken);
        return Results.Ok(sousFamilles);
    }
}

public record GetSousFamilleProduitsQueryParams(int? FamilleProduitId = null);

