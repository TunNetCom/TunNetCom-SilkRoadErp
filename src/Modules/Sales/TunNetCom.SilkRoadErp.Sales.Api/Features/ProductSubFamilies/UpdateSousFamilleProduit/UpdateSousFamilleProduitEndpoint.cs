using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.UpdateSousFamilleProduit;

public class UpdateSousFamilleProduitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/product-subfamilies/{id}",
            async Task<Results<NoContent, NotFound, BadRequest<List<IError>>>> (
                IMediator mediator,
                int id,
                UpdateSousFamilleProduitRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateSousFamilleProduitCommand(id, request.Nom, request.FamilleProduitId);
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
            .WithTags("ProductSubFamilies");
    }
}

