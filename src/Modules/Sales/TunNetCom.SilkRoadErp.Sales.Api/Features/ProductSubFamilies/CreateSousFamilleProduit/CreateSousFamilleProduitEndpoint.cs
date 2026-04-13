using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.CreateSousFamilleProduit;

public class CreateSousFamilleProduitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/product-subfamilies",
            async Task<Results<Created<SousFamilleProduitResponse>, ValidationProblem>> (
                IMediator mediator,
                CreateSousFamilleProduitRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateSousFamilleProduitCommand(request.Nom, request.FamilleProduitId);
                var result = await mediator.Send(command, cancellationToken);

                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                var famille = await mediator.Send(new GetFamilleProduitQuery(request.FamilleProduitId), cancellationToken);
                var response = new SousFamilleProduitResponse
                {
                    Id = result.Value,
                    Nom = request.Nom,
                    FamilleProduitId = request.FamilleProduitId,
                    FamilleProduitNom = famille?.Nom
                };

                return TypedResults.Created($"/product-subfamilies/{result.Value}", response);
            })
            .RequireAuthorization($"Permission:{Permissions.CreateProduct}")
            .WithTags("ProductSubFamilies");
    }
}

public record GetFamilleProduitQuery(int Id) : IRequest<FamilleProduitResponse?>;
public class GetFamilleProduitQueryHandler(SalesContext context) : IRequestHandler<GetFamilleProduitQuery, FamilleProduitResponse?>
{
    public async Task<FamilleProduitResponse?> Handle(GetFamilleProduitQuery request, CancellationToken cancellationToken)
    {
        var famille = await context.FamilleProduit
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);
        
        return famille == null ? null : new FamilleProduitResponse { Id = famille.Id, Nom = famille.Nom };
    }
}
