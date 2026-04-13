using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.CreateFamilleProduit;

public class CreateFamilleProduitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/product-families",
            async Task<Results<Created<FamilleProduitResponse>, ValidationProblem>> (
                IMediator mediator,
                CreateFamilleProduitRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateFamilleProduitCommand(request.Nom);
                var result = await mediator.Send(command, cancellationToken);

                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                var response = new FamilleProduitResponse
                {
                    Id = result.Value,
                    Nom = request.Nom
                };

                return TypedResults.Created($"/product-families/{result.Value}", response);
            })
            .RequireAuthorization($"Permission:{Permissions.CreateProduct}")
            .WithTags("ProductFamilies");
    }
}

