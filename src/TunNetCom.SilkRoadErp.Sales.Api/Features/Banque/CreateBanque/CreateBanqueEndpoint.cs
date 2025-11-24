using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.CreateBanque;

public class CreateBanqueEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/banque", HandleCreateBanqueAsync)
            .WithTags(EndpointTags.Banque);
    }

    public async Task<Results<Created<CreateBanqueRequest>, ValidationProblem>> HandleCreateBanqueAsync(
        IMediator mediator,
        CreateBanqueRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBanqueCommand(request.Nom);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/banque/{result.Value}", request);
    }
}

