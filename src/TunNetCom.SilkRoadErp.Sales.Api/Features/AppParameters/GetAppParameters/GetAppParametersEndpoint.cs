using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

public class GetAppParametersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/appParameters",
                async Task<IResult> (
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetAppParametersQuery();
                    var result = await mediator.Send(query, cancellationToken);
                    if (result.IsFailed)
                    {
                        return Results.BadRequest(result.Reasons);
                    }
                    return Results.Ok(result.Value);
                })
            .WithName("GetAppParameters")
            .Produces<GetAppParametersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
