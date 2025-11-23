using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.CreateAvoir;

public class CreateAvoirEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/avoirs", HandleCreateAvoirAsync)
            .WithTags(EndpointTags.Avoirs);
    }

    public async Task<Results<Created<CreateAvoirRequest>, ValidationProblem>> HandleCreateAvoirAsync(
        IMediator mediator,
        CreateAvoirRequest createAvoirRequest,
        CancellationToken cancellationToken)
    {
        var command = new CreateAvoirCommand(
            createAvoirRequest.Date,
            createAvoirRequest.ClientId,
            createAvoirRequest.Lines);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/avoirs/{result.Value}", createAvoirRequest);
    }
}

