using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.DeleteSousFamilleProduit;

public class DeleteSousFamilleProduitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/product-subfamilies/{id}",
            async Task<Results<NoContent, NotFound, BadRequest<List<IError>>>> (
                IMediator mediator,
                int id,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteSousFamilleProduitCommand(id);
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
            .RequireAuthorization($"Permission:{Permissions.DeleteProduct}")
            .WithTags("ProductSubFamilies");
    }
}

