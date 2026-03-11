using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.ValidateAvoirs;

public class ValidateAvoirsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/avoirs/validate", async ([FromBody] List<int> ids, ISender sender) =>
        {
            var command = new ValidateAvoirsCommand(ids);
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("Avoirs")
        .RequireAuthorization();
    }
}
