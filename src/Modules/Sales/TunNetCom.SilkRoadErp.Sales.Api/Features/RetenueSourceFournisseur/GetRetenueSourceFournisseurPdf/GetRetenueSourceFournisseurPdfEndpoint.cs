namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.GetRetenueSourceFournisseurPdf;

public class GetRetenueSourceFournisseurPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retenue-source-fournisseur/{numFactureFournisseur:int}/pdf", HandleGetRetenueSourceFournisseurPdfAsync)
            .WithTags("RetenueSourceFournisseur")
            .RequireAuthorization();
    }

    public async Task<Results<FileContentHttpResult, NotFound, ValidationProblem>> HandleGetRetenueSourceFournisseurPdfAsync(
        IMediator mediator,
        int numFactureFournisseur,
        CancellationToken cancellationToken)
    {
        var query = new GetRetenueSourceFournisseurPdfQuery(numFactureFournisseur);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.File(result.Value, "application/pdf", $"retenue_source_fournisseur_{numFactureFournisseur}.pdf");
    }
}

