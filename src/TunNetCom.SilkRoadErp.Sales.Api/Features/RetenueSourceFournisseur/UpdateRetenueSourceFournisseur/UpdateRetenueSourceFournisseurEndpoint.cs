using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.UpdateRetenueSourceFournisseur;

public class UpdateRetenueSourceFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/retenue-source-fournisseur/{numFactureFournisseur:int}", HandleUpdateRetenueSourceFournisseurAsync)
            .WithTags("RetenueSourceFournisseur")
            .RequireAuthorization();
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleUpdateRetenueSourceFournisseurAsync(
        IMediator mediator,
        int numFactureFournisseur,
        UpdateRetenueSourceFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRetenueSourceFournisseurCommand(
            numFactureFournisseur,
            request.NumTej,
            request.PdfContent);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}


