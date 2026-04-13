using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.GetRetenueSourceFournisseur;

public class GetRetenueSourceFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retenue-source-fournisseur/{numFactureFournisseur:int}", HandleGetRetenueSourceFournisseurAsync)
            .WithTags("RetenueSourceFournisseur")
            .RequireAuthorization();
    }

    public async Task<Results<Ok<RetenueSourceFournisseurResponse>, NotFound, ValidationProblem>> HandleGetRetenueSourceFournisseurAsync(
        IMediator mediator,
        int numFactureFournisseur,
        CancellationToken cancellationToken)
    {
        var query = new GetRetenueSourceFournisseurQuery(numFactureFournisseur);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}


