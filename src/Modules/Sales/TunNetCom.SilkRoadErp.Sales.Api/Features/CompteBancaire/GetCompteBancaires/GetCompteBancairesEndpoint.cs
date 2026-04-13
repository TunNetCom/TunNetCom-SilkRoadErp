using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.GetCompteBancaires;

public class GetCompteBancairesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/compte-bancaire", HandleGetAsync)
            .WithTags(EndpointTags.CompteBancaire);
    }

    public async Task<Ok<List<CompteBancaireResponse>>> HandleGetAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCompteBancairesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}
