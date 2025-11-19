namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;

public class GetFullInvoiceByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/{id:int}/full", HandleGetFullInvoiceByIdAsync)
            .WithTags(EndpointTags.Invoices);
    }

    public static async Task<Results<Ok<FullInvoiceResponse>, NotFound>> HandleGetFullInvoiceByIdAsync(
        IMediator mediator, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFullInvoiceByIdQuery(id), cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}