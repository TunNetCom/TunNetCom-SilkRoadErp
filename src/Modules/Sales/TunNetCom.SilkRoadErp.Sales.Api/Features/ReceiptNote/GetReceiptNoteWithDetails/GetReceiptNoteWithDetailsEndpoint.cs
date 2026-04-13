using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes.Request;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails
{
    public class GetReceiptNoteWithDetailsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            _ = app.MapGet(
                "/receipt_note/summaries",
                static async Task<Results<Ok<ReceiptNotesWithSummaryResponse>, BadRequest<ProblemDetails>>> (
                    IMediator mediator,
                    [AsParameters] GetReceipNotesQueryParams getReceipNotesQueryParams,
                    CancellationToken cancellationToken) =>
                {
                    var queryStringParams = new QueryStringParameters
                    {
                        PageNumber = getReceipNotesQueryParams.PageNumber,
                        PageSize = getReceipNotesQueryParams.PageSize,
                        SortProprety = getReceipNotesQueryParams.SortProprety,
                        SortOrder = getReceipNotesQueryParams.SortOrder,
                        SearchKeyword = getReceipNotesQueryParams.SearchKeyword,
                        StartDate = getReceipNotesQueryParams.StartDate,
                        EndDate = getReceipNotesQueryParams.EndDate
                    };

                    int? providerIdValue = null;
                    if (!string.IsNullOrWhiteSpace(getReceipNotesQueryParams.ProviderId) && int.TryParse(getReceipNotesQueryParams.ProviderId, out var parsedId))
                    {
                        providerIdValue = parsedId;
                    }

                    var query = new GetReceiptNotesWithSummaryQuery(
                        queryStringParameters: queryStringParams,
                        IdFournisseur: providerIdValue,
                        IsInvoiced: getReceipNotesQueryParams.IsInvoiced,
                        InvoiceId: getReceipNotesQueryParams.InvoiceId);

                    var response = await mediator.Send(query, cancellationToken);

                    return TypedResults.Ok(response);

                })
            .WithName("GetReceiptNote")
            .WithTags(EndpointTags.ReceiptNotes)
            .Produces<ReceiptNotesWithSummaryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Gets a paginated list of receipt notes with details and summary totals.");
        }
    }
}
