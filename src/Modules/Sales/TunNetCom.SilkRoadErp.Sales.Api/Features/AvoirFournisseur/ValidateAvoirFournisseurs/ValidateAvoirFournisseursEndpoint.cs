using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.ValidateAvoirFournisseurs;

public class ValidateAvoirFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/avoir-fournisseurs/validate", async ([FromBody] List<int> ids, ISender sender) =>
        {
            var command = new ValidateAvoirFournisseursCommand(ids);
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("AvoirFournisseur")
        .RequireAuthorization();
    }
}
