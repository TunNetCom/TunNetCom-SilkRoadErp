using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Contracts;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementsFournisseur;

public class GetPaiementsFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiement-fournisseur", HandleGetPaiementsFournisseurAsync)
            .WithTags(EndpointTags.PaiementFournisseur);
    }

    public async Task<Ok<PagedList<PaiementFournisseurResponse>>> HandleGetPaiementsFournisseurAsync(
        IMediator mediator,
        [AsParameters] GetPaiementsFournisseurQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        var query = new GetPaiementsFournisseurQuery(
            queryParams.FournisseurId,
            queryParams.AccountingYearIds,
            queryParams.DateEcheanceFrom,
            queryParams.DateEcheanceTo,
            queryParams.MontantMin,
            queryParams.MontantMax,
            queryParams.HasNumeroTransactionBancaire,
            queryParams.Mois,
            queryParams.PageNumber,
            queryParams.PageSize);

        var result = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(result.Value);
    }
}

public class GetPaiementsFournisseurQueryParams : QueryStringParameters
{
    public int? FournisseurId { get; set; }
    public int[]? AccountingYearIds { get; set; }
    public DateTime? DateEcheanceFrom { get; set; }
    public DateTime? DateEcheanceTo { get; set; }
    public decimal? MontantMin { get; set; }
    public decimal? MontantMax { get; set; }
    public bool? HasNumeroTransactionBancaire { get; set; }
    public int? Mois { get; set; }
}

