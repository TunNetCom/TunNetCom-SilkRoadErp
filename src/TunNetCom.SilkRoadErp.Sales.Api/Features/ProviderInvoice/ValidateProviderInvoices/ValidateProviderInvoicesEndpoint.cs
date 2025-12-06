using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.ValidateProviderInvoices;

public class ValidateProviderInvoicesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/provider-invoices/validate", async (ValidateProviderInvoicesRequest request, ISender sender) =>
        {
            var command = new ValidateProviderInvoicesCommand(request.Ids);
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithTags("ProviderInvoice")
        .RequireAuthorization();
    }
}
