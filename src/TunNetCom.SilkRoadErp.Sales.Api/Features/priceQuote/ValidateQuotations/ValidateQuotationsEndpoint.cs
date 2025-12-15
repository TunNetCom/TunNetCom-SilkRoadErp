using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.ValidateQuotations;

public class ValidateQuotationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/quotations/validate", async (ValidateQuotationsRequest request, ISender sender) =>
        {
            var command = new ValidateQuotationsCommand(request.Ids);
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("Quotations")
        .RequireAuthorization();
    }
}
