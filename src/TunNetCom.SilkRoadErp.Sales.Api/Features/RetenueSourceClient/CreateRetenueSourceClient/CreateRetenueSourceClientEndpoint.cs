using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.CreateRetenueSourceClient;

public class CreateRetenueSourceClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/retenue-source-client", HandleCreateRetenueSourceClientAsync)
            .WithTags("RetenueSourceClient")
            .RequireAuthorization();
    }

    public async Task<Results<Created<RetenueSourceClientResponse>, ValidationProblem>> HandleCreateRetenueSourceClientAsync(
        IMediator mediator,
        CreateRetenueSourceClientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetenueSourceClientCommand(
            request.NumFacture,
            request.NumTej,
            request.PdfContent);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/retenue-source-client/{result.Value}", new RetenueSourceClientResponse { Id = result.Value });
    }
}


