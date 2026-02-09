using Microsoft.AspNetCore.Http.HttpResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.DeleteCompteBancaire;

public class DeleteCompteBancaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/compte-bancaire/{id:int}", HandleDeleteAsync)
            .WithTags(EndpointTags.CompteBancaire);
    }

    public async Task<Results<NoContent, ValidationProblem, NotFound>> HandleDeleteAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCompteBancaireCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message == "compte_bancaire_not_found"))
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
