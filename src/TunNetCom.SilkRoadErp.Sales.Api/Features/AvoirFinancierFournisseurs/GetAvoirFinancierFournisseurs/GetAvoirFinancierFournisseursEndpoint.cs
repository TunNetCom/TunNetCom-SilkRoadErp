using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseurs;

public class GetAvoirFinancierFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoir-financier-fournisseurs/{num:int}", HandleGetAvoirFinancierFournisseursAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs);
    }

    public async Task<Results<Ok<AvoirFinancierFournisseursResponse>, NotFound>> HandleGetAvoirFinancierFournisseursAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var query = new GetAvoirFinancierFournisseursQuery(num);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

