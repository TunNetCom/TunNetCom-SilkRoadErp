using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.ValidateDeliveryNotes;

public class ValidateDeliveryNotesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/delivery-notes/validate", async (ValidateDeliveryNotesCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("DeliveryNotes")
        .RequireAuthorization();
    }
}