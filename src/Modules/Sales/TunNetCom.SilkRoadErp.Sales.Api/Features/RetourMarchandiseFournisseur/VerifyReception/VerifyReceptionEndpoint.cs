using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;
using System.Security.Claims;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.VerifyReception;

public class VerifyReceptionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/retour-marchandise-fournisseur/verify-reception",
            async ([FromBody] VerifyReceptionRequest request,
            IMediator mediator,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            // Récupérer le nom de l'utilisateur depuis les claims
            var utilisateur = user.Identity?.Name ?? user.FindFirst(ClaimTypes.Email)?.Value ?? "Système";

            var command = new VerifyReceptionCommand(
                Num: request.Num,
                Lines: request.Lines,
                Utilisateur: utilisateur,
                Commentaire: request.Commentaire);

            var result = await mediator.Send(command, cancellationToken);
            
            if (result.IsFailed)
            {
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            }

            return Results.Ok(result.Value);
        })
        .WithTags(EndpointTags.RetourMarchandiseFournisseur)
        .WithName("VerifyReception")
        .WithSummary("Verify reception of repaired items from supplier return")
        .Produces<VerifyReceptionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
