using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.CreateRetenueSourceFournisseur;

public class CreateRetenueSourceFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/retenue-source-fournisseur", HandleCreateRetenueSourceFournisseurAsync)
            .WithTags("RetenueSourceFournisseur")
            .RequireAuthorization();
    }

    public async Task<Results<Created<RetenueSourceFournisseurResponse>, ValidationProblem>> HandleCreateRetenueSourceFournisseurAsync(
        IMediator mediator,
        CreateRetenueSourceFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetenueSourceFournisseurCommand(
            request.NumFactureFournisseur,
            request.NumTej,
            request.PdfContent);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/retenue-source-fournisseur/{result.Value}", new RetenueSourceFournisseurResponse { Id = result.Value });
    }
}


