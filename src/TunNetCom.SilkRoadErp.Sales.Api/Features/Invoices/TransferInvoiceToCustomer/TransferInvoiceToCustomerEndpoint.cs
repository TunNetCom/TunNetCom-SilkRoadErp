namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.TransferInvoiceToCustomer;

public class TransferInvoiceToCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/invoices/{invoiceNumber}/transfer", HandleTransferInvoiceAsync)
            .RequireAuthorization($"Permission:{Permissions.UpdateInvoice}")
            .WithTags(EndpointTags.Invoices);
    }

    public async Task<Results<Ok, ValidationProblem>> HandleTransferInvoiceAsync(
        IMediator mediator,
        int invoiceNumber,
        TransferInvoiceToCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new TransferInvoiceToCustomerCommand(invoiceNumber, request.TargetCustomerId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok();
    }
}

