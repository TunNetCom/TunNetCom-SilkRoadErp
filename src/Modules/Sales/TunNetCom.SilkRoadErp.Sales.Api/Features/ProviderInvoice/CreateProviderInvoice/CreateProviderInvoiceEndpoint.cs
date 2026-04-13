using MediatR;
using Carter;
using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.CreateProviderInvoice;

public class CreateProviderInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/provider-invoice", HandleCreateProviderInvoiceAsync)
            .WithTags(EndpointTags.ProviderInvoices);
    }
    
    public async Task<Results<Created<CreateProviderInvoiceRequest>, ValidationProblem>> HandleCreateProviderInvoiceAsync(
        IMediator mediator,
        CreateProviderInvoiceRequest createProviderInvoiceRequest,
        CancellationToken cancellationToken)
    {
        var command = new CreateProviderInvoiceCommand(createProviderInvoiceRequest.Date, createProviderInvoiceRequest.ProviderId, createProviderInvoiceRequest.NumFactureFournisseur);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/provider-invoice/{result.Value}", createProviderInvoiceRequest);
    }
}

