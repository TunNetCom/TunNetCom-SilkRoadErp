using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.AttachAvoirFinancierToInvoice;

public class AttachAvoirFinancierToInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/avoir-financier-fournisseurs/{id:int}/attach", HandleAttachAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs)
            .WithDescription("Attaches an avoir financier fournisseur to a provider invoice.");
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleAttachAsync(
        IMediator mediator,
        int id,
        AttachAvoirFinancierToInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AttachAvoirFinancierToInvoiceCommand(id, request.NumFactureFournisseur);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
