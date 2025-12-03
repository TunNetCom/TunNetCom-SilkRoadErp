namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.GetRetenueSourceClientPdf;

public class GetRetenueSourceClientPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retenue-source-client/{numFacture:int}/pdf", HandleGetRetenueSourceClientPdfAsync)
            .WithTags("RetenueSourceClient")
            .RequireAuthorization();
    }

    public async Task<Results<FileContentHttpResult, NotFound, ValidationProblem>> HandleGetRetenueSourceClientPdfAsync(
        IMediator mediator,
        int numFacture,
        CancellationToken cancellationToken)
    {
        var query = new GetRetenueSourceClientPdfQuery(numFacture);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.File(result.Value, "application/pdf", $"retenue_source_client_{numFacture}.pdf");
    }
}

