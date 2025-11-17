using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetFullProviderInvoice;

public class GetFullProviderInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/provider-invoices/{id:int}/full", HandleGetFullProviderInvoiceByIdAsync)
            .WithTags(SwaggerTags.ProviderInvoices);
    }
    public static async Task<Results<Ok<FullProviderInvoiceResponse>, NotFound>> HandleGetFullProviderInvoiceByIdAsync(
        IMediator mediator, int id, CancellationToken cancellationToken)
    {
        var query = new GetFullProviderInvoiceQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.IsEntityNotFound() ? TypedResults.NotFound() : TypedResults.Ok(result.Value);
    }
}
