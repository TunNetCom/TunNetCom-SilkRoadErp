using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;

public class GetDeliveryNotesBasedOnProductReferenceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/deliveryNoteHistory", async (
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

            var query = new GetDeliveryNotesBasedOnProductReferenceQuery(
                productReference.Trim(),
                paginationQueryParams.PageNumber,
                paginationQueryParams.PageSize);

            var pagedDeliveryNotes = await mediator.Send(query, cancellationToken);

            var metadata = new
            {
                pagedDeliveryNotes.TotalCount,
                pagedDeliveryNotes.PageSize,
                pagedDeliveryNotes.CurrentPage,
                pagedDeliveryNotes.TotalPages,
                pagedDeliveryNotes.HasNext,
                pagedDeliveryNotes.HasPrevious
            };
            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

            return Results.Ok(pagedDeliveryNotes);
        })
            .WithName("GetDeliveryNotesByProductReference")
            .WithTags(EndpointTags.DeliveryNotes)
            .Produces<PagedList<DeliveryNoteDetailResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}