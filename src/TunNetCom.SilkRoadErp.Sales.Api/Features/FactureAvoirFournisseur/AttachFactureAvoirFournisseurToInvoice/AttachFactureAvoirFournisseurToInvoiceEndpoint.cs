using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.AttachFactureAvoirFournisseurToInvoice;

public class AttachFactureAvoirFournisseurToInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut(
                "/facture-avoir-fournisseur/attach-to-invoice",
                async Task<Results<NoContent, NotFound, ValidationProblem>> (
                    IMediator mediator,
                    [FromBody] AttachFactureAvoirFournisseurToInvoiceRequest request,
                    CancellationToken cancellationToken) =>
                {
                    var command = new AttachFactureAvoirFournisseurToInvoiceCommand(
                        request.FactureAvoirFournisseurIds,
                        request.FactureFournisseurId);

                    var result = await mediator.Send(command, cancellationToken);

                    if (result.HasError<EntityNotFound>())
                    {
                        return TypedResults.NotFound();
                    }

                    if (result.IsFailed)
                    {
                        return result.ToValidationProblem();
                    }

                    return TypedResults.NoContent();
                })
            .WithName("AttachFactureAvoirFournisseurToInvoice")
            .WithTags(EndpointTags.FactureAvoirFournisseur)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status404NotFound, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Attaches facture avoir fournisseur to a facture fournisseur.");
    }
}







