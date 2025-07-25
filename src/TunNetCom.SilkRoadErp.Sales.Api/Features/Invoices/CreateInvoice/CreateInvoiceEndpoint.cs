﻿namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;

public class CreateInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/invoices", HandleCreateInvoiceAsync);
    }
    public async Task<Results<Created<CreateInvoiceRequest>, ValidationProblem>> HandleCreateInvoiceAsync(
        IMediator mediator,
        CreateInvoiceRequest createInvoiceRequest,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(createInvoiceRequest.Date, createInvoiceRequest.ClientId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/invoices/{result.Value}", createInvoiceRequest);
    }
}