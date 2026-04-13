using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.CreateRetenueSourceFactureDepense;

public class CreateRetenueSourceFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/retenue-source-facture-depense", HandleCreateRetenueSourceFactureDepenseAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense);
    }

    public static async Task<Results<Created, ValidationProblem>> HandleCreateRetenueSourceFactureDepenseAsync(
        IMediator mediator,
        CreateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetenueSourceFactureDepenseCommand(
            request.FactureDepenseId,
            request.NumTej,
            request.PdfContent);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/retenue-source-facture-depense/{result.Value}");
    }
}
