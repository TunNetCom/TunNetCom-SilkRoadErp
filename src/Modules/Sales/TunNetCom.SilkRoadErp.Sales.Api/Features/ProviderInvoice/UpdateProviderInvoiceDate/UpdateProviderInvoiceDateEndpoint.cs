using Carter;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.UpdateProviderInvoiceDate;

public class UpdateProviderInvoiceDateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/provider-invoice/{num:int}/date",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                int num,
                [FromBody] UpdateProviderInvoiceDateRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateProviderInvoiceDateCommand(num, request.Date);
                var result = await mediator.Send(command, cancellationToken);

                if (result.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                return TypedResults.NoContent();
            })
            .RequireAuthorization($"Permission:{Permissions.UpdateProviderInvoice}")
            .WithTags(EndpointTags.ProviderInvoices)
            .WithName("UpdateProviderInvoiceDate")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest);
    }
}
