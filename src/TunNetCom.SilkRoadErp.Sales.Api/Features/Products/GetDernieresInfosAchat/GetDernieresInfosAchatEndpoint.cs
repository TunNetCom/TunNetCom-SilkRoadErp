using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetDernieresInfosAchat;

public class GetDernieresInfosAchatEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/products/dernieres-infos-achat/{refProduit}",
            async (IMediator mediator, string refProduit, CancellationToken cancellationToken) =>
            {
                var query = new GetDernieresInfosAchatQuery(RefProduit: refProduit);
                var result = await mediator.Send(query, cancellationToken);
                
                if (result.IsFailed)
                {
                    return Results.NotFound();
                }
                
                return Results.Ok(result.Value);
            })
            .WithTags("Products");
    }
}


