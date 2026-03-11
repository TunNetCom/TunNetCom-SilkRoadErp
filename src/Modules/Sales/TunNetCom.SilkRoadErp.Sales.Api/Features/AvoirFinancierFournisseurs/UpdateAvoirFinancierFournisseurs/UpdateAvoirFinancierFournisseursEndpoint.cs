using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.UpdateAvoirFinancierFournisseurs;

public class UpdateAvoirFinancierFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/avoir-financier-fournisseurs/{num:int}", HandleUpdateAvoirFinancierFournisseursAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleUpdateAvoirFinancierFournisseursAsync(
        IMediator mediator,
        int num,
        UpdateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAvoirFinancierFournisseursCommand(
            num,
            request.NumSurPage,
            request.Date,
            request.Description,
            request.TotTtc);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

