using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.ValidateRetourMarchandiseFournisseur;

public class ValidateRetourMarchandiseFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/retour-marchandise-fournisseur/validate",
            async ([FromBody] ValidateRetourMarchandiseFournisseurRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new ValidateRetourMarchandiseFournisseurCommand(request.Ids);
            var result = await mediator.Send(command, cancellationToken);
            
            if (result.IsFailed)
            {
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            }
            
            return Results.Ok();
        })
        .WithTags(EndpointTags.RetourMarchandiseFournisseur)
        .WithName("ValidateRetourMarchandiseFournisseur")
        .WithSummary("Validate one or more supplier returns")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

