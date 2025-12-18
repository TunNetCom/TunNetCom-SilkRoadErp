using MediatR;
using Carter;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.UpdateProviderInvoice;

public class UpdateProviderInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/provider-invoice/{num:int}",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                int num,
                UpdateProviderInvoiceRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateProviderInvoiceCommand(num, request.NumFactureFournisseur);
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
            .WithName("UpdateProviderInvoice")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest)
            .WithDescription("Updates the supplier invoice number (NumFactureFournisseur) for a provider invoice.");
    }
}

