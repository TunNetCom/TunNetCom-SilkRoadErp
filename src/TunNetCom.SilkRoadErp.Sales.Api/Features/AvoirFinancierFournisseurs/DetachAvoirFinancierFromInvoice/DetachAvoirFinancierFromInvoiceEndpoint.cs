using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.DetachAvoirFinancierFromInvoice;

public class DetachAvoirFinancierFromInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/avoir-financier-fournisseurs/{id:int}/detach", HandleDetachAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs)
            .WithDescription("Detaches an avoir financier fournisseur from its provider invoice.");
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDetachAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DetachAvoirFinancierFromInvoiceCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
