using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNotesBasedOnProductReference;

public class GetReceiptNotesBasedOnProductReferenceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/receiptNoteHistory", async (
                IMediator mediator,
                [FromQuery] string? productReference,
                [AsParameters] QueryStringParameters paginationQueryParams,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(productReference))
            {
                return Results.BadRequest("Product reference cannot be null or empty.");
            }

            var query = new GetReceiptNotesBasedOnProductReferenceQuery(
                productReference.Trim(),
                paginationQueryParams.PageNumber,
                paginationQueryParams.PageSize);

            var pagedReceiptNotes = await mediator.Send(query, cancellationToken);

            var metadata = new
            {
                pagedReceiptNotes.TotalCount,
                pagedReceiptNotes.PageSize,
                pagedReceiptNotes.CurrentPage,
                pagedReceiptNotes.TotalPages,
                pagedReceiptNotes.HasNext,
                pagedReceiptNotes.HasPrevious
            };
            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

            return Results.Ok(pagedReceiptNotes);
        })
            .WithName("GetReceiptNotesByProductReference")
            .WithTags(EndpointTags.ReceiptNotes)
            .Produces<PagedList<ReceiptNoteDetailResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}

