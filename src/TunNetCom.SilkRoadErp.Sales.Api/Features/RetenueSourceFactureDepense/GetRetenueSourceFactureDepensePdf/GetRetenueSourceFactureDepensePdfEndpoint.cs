using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.GetRetenueSourceFactureDepensePdf;

public class GetRetenueSourceFactureDepensePdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retenue-source-facture-depense/{factureDepenseId:int}/pdf", HandleGetRetenueSourceFactureDepensePdfAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense);
    }

    public static async Task<Results<FileContentHttpResult, NotFound, ValidationProblem>> HandleGetRetenueSourceFactureDepensePdfAsync(
        IMediator mediator,
        int factureDepenseId,
        CancellationToken cancellationToken)
    {
        var query = new GetRetenueSourceFactureDepensePdfQuery(factureDepenseId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.File(result.Value, "application/pdf", $"retenue_source_facture_depense_{factureDepenseId}.pdf");
    }
}
