using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.UpdateRetenueSourceClient;

public class UpdateRetenueSourceClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/retenue-source-client/{numFacture:int}", HandleUpdateRetenueSourceClientAsync)
            .WithTags("RetenueSourceClient")
            .RequireAuthorization();
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleUpdateRetenueSourceClientAsync(
        IMediator mediator,
        int numFacture,
        UpdateRetenueSourceClientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRetenueSourceClientCommand(
            numFacture,
            request.NumTej,
            request.PdfContent);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}


