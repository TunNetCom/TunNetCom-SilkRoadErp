using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.UpdateAvoir;

public class UpdateAvoirEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/avoirs/{num:int}", HandleUpdateAvoirAsync)
            .WithTags(EndpointTags.Avoirs);
    }

    public async Task<Results<NoContent, NotFound, ValidationProblem>> HandleUpdateAvoirAsync(
        IMediator mediator,
        int num,
        UpdateAvoirRequest updateAvoirRequest,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAvoirCommand(
            num,
            updateAvoirRequest.Date,
            updateAvoirRequest.ClientId,
            updateAvoirRequest.Lines);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsEntityNotFound())
        {
            return TypedResults.NotFound();
        }

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

