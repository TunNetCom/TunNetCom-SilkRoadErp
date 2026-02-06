using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.DeleteAvoirFinancierFournisseurs;

public class DeleteAvoirFinancierFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/avoir-financier-fournisseurs/{id:int}", HandleDeleteAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs)
            .WithDescription("Deletes an avoir financier fournisseur by Id.");
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeleteAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAvoirFinancierFournisseursCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
