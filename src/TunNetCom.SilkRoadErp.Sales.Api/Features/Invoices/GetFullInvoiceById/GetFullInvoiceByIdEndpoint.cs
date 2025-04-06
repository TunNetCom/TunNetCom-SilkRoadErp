namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;

public class GetFullInvoiceByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/invoices/{id:int}/full", HandleGetFullInvoiceByIdAsync);
    }

    public static async Task<Results<Ok<FullInvoiceResponse>, NotFound>> HandleGetFullInvoiceByIdAsync(
        IMediator mediator, int id, CancellationToken cancellationToken)
    {
        var query = new GetFullInvoiceByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsEntityNotFound() ? TypedResults.NotFound() : TypedResults.Ok(result.Value);
    }
}