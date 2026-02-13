using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.GetRetenueSourceFactureDepense;

public class GetRetenueSourceFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retenue-source-facture-depense/{factureDepenseId:int}", HandleGetRetenueSourceFactureDepenseAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense);
    }

    public static async Task<Results<Ok<RetenueSourceFactureDepenseResponse>, NotFound, ValidationProblem>> HandleGetRetenueSourceFactureDepenseAsync(
        IMediator mediator,
        int factureDepenseId,
        CancellationToken cancellationToken)
    {
        var query = new GetRetenueSourceFactureDepenseQuery(factureDepenseId);
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
