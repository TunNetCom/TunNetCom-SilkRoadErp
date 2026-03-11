namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceIdByNumber;

public class GetInvoiceIdByNumberEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/{number:int}/id", HandleGetInvoiceIdByNumberAsync)
            .WithTags(EndpointTags.Invoices)
            .WithDescription("Get invoice ID by invoice number")
            .Produces<int>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<Results<Ok<int>, NotFound>> HandleGetInvoiceIdByNumberAsync(
        IMediator mediator, int number, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvoiceIdByNumberQuery(number), cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}
