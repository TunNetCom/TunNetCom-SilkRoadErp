using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.Contracts;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementsClient;

public class GetPaiementsClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiement-client", HandleGetPaiementsClientAsync)
            .WithTags(EndpointTags.PaiementClient);
    }

    public async Task<Ok<PagedList<PaiementClientResponse>>> HandleGetPaiementsClientAsync(
        IMediator mediator,
        [AsParameters] GetPaiementsClientQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        var query = new GetPaiementsClientQuery(
            queryParams.ClientId,
            queryParams.AccountingYearId,
            queryParams.DateEcheanceFrom,
            queryParams.DateEcheanceTo,
            queryParams.MontantMin,
            queryParams.MontantMax,
            queryParams.PageNumber,
            queryParams.PageSize);

        var result = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(result.Value);
    }
}

public class GetPaiementsClientQueryParams : QueryStringParameters
{
    public int? ClientId { get; set; }
    public int? AccountingYearId { get; set; }
    public DateTime? DateEcheanceFrom { get; set; }
    public DateTime? DateEcheanceTo { get; set; }
    public decimal? MontantMin { get; set; }
    public decimal? MontantMax { get; set; }
}

