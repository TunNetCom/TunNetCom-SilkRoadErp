using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.ValidateFactureAvoirFournisseurs;

public class ValidateFactureAvoirFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/facture-avoir-fournisseurs/validate", async (ValidateFactureAvoirFournisseursCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("FactureAvoirFournisseur")
        .RequireAuthorization();
    }
}
