using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.DetachFactureAvoirFournisseurFromInvoice;

public class DetachFactureAvoirFournisseurFromInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut(
                "/facture-avoir-fournisseur/detach-from-invoice",
                async Task<Results<NoContent, NotFound, ValidationProblem>> (
                    IMediator mediator,
                    [FromBody] DetachFactureAvoirFournisseurFromInvoiceRequest request,
                    CancellationToken cancellationToken) =>
                {
                    var command = new DetachFactureAvoirFournisseurFromInvoiceCommand(
                        request.FactureFournisseurId,
                        request.FactureAvoirFournisseurIds);

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
            .WithName("DetachFactureAvoirFournisseurFromInvoice")
            .WithTags(EndpointTags.FactureAvoirFournisseur)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status404NotFound, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Detaches facture avoir fournisseur from a facture fournisseur.");
    }
}


