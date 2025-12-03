using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.GetRetenueSourceClient;

public class GetRetenueSourceClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retenue-source-client/{numFacture:int}", HandleGetRetenueSourceClientAsync)
            .WithTags("RetenueSourceClient")
            .RequireAuthorization();
    }

    public async Task<Results<Ok<RetenueSourceClientResponse>, NotFound, ValidationProblem>> HandleGetRetenueSourceClientAsync(
        IMediator mediator,
        int numFacture,
        CancellationToken cancellationToken)
    {
        var query = new GetRetenueSourceClientQuery(numFacture);
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


