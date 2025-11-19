using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithIds;

public class GetInvoicesByIdsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
                "/invoices/byids",
                async Task<IResult> (
                    IMediator mediator,
                    [FromBody] List<int> invoicesIds,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetInvoicesWithIdsQuery(invoicesIds);
                    var result = await mediator.Send(query, cancellationToken);
                    if (result.IsFailed)
                    {
                        return Results.BadRequest(result.Reasons);
                    }
                    return Results.Ok(result.Value);
                })
            .WithName("GetInvoicesByIds")
            .WithTags(EndpointTags.Invoices)
            .Produces<IList<InvoiceResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}