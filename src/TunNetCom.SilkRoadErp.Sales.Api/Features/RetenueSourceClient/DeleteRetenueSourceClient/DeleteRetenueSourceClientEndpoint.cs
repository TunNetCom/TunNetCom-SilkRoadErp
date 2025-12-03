namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.DeleteRetenueSourceClient;

public class DeleteRetenueSourceClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/retenue-source-client/{numFacture:int}", HandleDeleteRetenueSourceClientAsync)
            .WithTags("RetenueSourceClient")
            .RequireAuthorization();
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeleteRetenueSourceClientAsync(
        IMediator mediator,
        int numFacture,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRetenueSourceClientCommand(numFacture);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}


