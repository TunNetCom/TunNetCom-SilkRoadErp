using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.ValidateProviderInvoices;

public class ValidateProviderInvoicesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/provider-invoices/validate", async (ValidateProviderInvoicesCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("ProviderInvoice")
        .RequireAuthorization();
    }
}