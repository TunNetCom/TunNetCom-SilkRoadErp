using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.CreateAvoirFinancierFournisseurs;

public class CreateAvoirFinancierFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/avoir-financier-fournisseurs", HandleCreateAvoirFinancierFournisseursAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs);
    }

    public async Task<Results<Created<CreateAvoirFinancierFournisseursRequest>, ValidationProblem>> HandleCreateAvoirFinancierFournisseursAsync(
        IMediator mediator,
        CreateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAvoirFinancierFournisseursCommand(
            request.NumFactureFournisseur,
            request.NumSurPage,
            request.Date,
            request.Description,
            request.TotTtc);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/avoir-financier-fournisseurs/{result.Value}", request);
    }
}

