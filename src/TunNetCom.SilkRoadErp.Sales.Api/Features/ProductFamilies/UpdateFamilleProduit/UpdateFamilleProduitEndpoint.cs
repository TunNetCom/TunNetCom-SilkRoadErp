using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.UpdateFamilleProduit;

public class UpdateFamilleProduitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/product-families/{id}",
            async Task<Results<NoContent, NotFound, BadRequest<List<IError>>>> (
                IMediator mediator,
                int id,
                UpdateFamilleProduitRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateFamilleProduitCommand(id, request.Nom);
                var result = await mediator.Send(command, cancellationToken);

                if (result.IsEntityNotFound())
                {
                    return TypedResults.NotFound();
                }

                if (result.IsFailed)
                {
                    return TypedResults.BadRequest(result.Errors);
                }

                return TypedResults.NoContent();
            })
            .RequireAuthorization($"Permission:{Permissions.UpdateProduct}")
            .WithTags("ProductFamilies");
    }
}

