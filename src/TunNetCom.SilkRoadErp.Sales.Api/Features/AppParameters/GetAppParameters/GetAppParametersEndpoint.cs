using System.Threading;
using System.Threading.Tasks;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

public class GetAppParametersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/appParameters", Handle)
           .WithName("GetAppParameters")
           .Produces<GetAppParametersResponse>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status400BadRequest);
    }

    // Méthode extraite pour test unitaire
    public async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetAppParametersQuery();
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return Results.BadRequest(result.Reasons);

        return Results.Ok(result.Value);
    }
}
