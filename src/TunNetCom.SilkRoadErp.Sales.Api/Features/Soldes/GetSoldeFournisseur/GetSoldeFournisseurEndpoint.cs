using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeFournisseur;

public class GetSoldeFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/soldes/fournisseur/{fournisseurId:int}", HandleGetSoldeFournisseurAsync)
            .WithTags(EndpointTags.Soldes);
    }

    public async Task<Results<Ok<SoldeFournisseurResponse>, NotFound>> HandleGetSoldeFournisseurAsync(
        IMediator mediator,
        int fournisseurId,
        int? accountingYearId,
        CancellationToken cancellationToken)
    {
        var query = new GetSoldeFournisseurQuery(fournisseurId, accountingYearId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

