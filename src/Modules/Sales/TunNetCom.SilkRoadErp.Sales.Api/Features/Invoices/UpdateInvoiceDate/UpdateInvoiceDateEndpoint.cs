using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using FluentResults;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.UpdateInvoiceDate;

public class UpdateInvoiceDateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/invoices/{num:int}/date",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                int num,
                [FromBody] UpdateInvoiceDateRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateInvoiceDateCommand(num, request.Date);
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
            .WithTags("Invoices")
            .WithName("UpdateInvoiceDate")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest);
    }
}
