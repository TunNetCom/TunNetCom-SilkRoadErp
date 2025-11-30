using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirClient.ValidateFactureAvoirClients;

public class ValidateFactureAvoirClientsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/facture-avoir-clients/validate", async (ValidateFactureAvoirClientsCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("FactureAvoirClient")
        .RequireAuthorization();
    }
}