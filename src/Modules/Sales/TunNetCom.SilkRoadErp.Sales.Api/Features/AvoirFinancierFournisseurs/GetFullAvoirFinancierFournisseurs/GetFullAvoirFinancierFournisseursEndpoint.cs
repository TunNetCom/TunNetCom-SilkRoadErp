using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetFullAvoirFinancierFournisseurs;

public class GetFullAvoirFinancierFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoir-financier-fournisseurs/{num:int}/full", HandleGetFullAvoirFinancierFournisseursAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs);
    }

    public async Task<Results<Ok<FullAvoirFinancierFournisseursResponse>, NotFound>> HandleGetFullAvoirFinancierFournisseursAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var query = new GetFullAvoirFinancierFournisseursQuery(num);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

