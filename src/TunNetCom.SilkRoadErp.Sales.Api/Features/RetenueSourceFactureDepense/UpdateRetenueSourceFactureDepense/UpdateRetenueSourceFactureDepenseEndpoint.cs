using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.UpdateRetenueSourceFactureDepense;

public class UpdateRetenueSourceFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/retenue-source-facture-depense/{factureDepenseId:int}", HandleUpdateRetenueSourceFactureDepenseAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense);
    }

    public static async Task<Results<NoContent, NotFound, ValidationProblem>> HandleUpdateRetenueSourceFactureDepenseAsync(
        IMediator mediator,
        int factureDepenseId,
        UpdateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRetenueSourceFactureDepenseCommand(
            factureDepenseId,
            request.NumTej,
            request.PdfContent);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
