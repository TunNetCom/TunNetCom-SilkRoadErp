namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails;
public class GetReceiptNoteWithDetailsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/receipt-note",
            static async Task<Results<Ok<ReceiptNotesWithSummary>, BadRequest<ProblemDetails>>> (
                IMediator mediator,
                [FromQuery] int PageNumber,
                [FromQuery] int PageSize,
                [FromQuery] string? SortProprety,
                [FromQuery] string? SortOrder,
                [FromQuery] string? SearchKeyword,
                [FromQuery] int ProviderId,
                [FromQuery] bool IsInvoiced,
                [FromQuery] int? InvoiceId,
                CancellationToken cancellationToken) =>
            {
                var queryStringParams = new QueryStringParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SortProprety = SortProprety,
                    SortOrder = SortOrder,
                    SearchKeyword = SearchKeyword
                };

                var query = new GetReceiptNoteWithDetailsQuery(
                    queryStringParameters: queryStringParams,
                    IdFournisseur: ProviderId,
                    IsInvoiced: IsInvoiced,
                    InvoiceId: InvoiceId);

                var response = await mediator.Send(query, cancellationToken);

                return TypedResults.Ok(response);

            })
        .WithName("GetReceiptNote")
        .Produces<ReceiptNotesWithSummary>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
        .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
        .WithDescription("Gets a paginated list of receipt notes with details and summary totals.");
    }
}
