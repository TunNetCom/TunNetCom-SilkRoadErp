namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;

public class GetDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/deliveryNote", async (
            [AsParameters] QueryStringParameters paginationQueryParams,
            bool? isFactured,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDeliveryNoteQuery(
                paginationQueryParams.PageNumber,
                paginationQueryParams.PageSize,
                paginationQueryParams.SearchKeyword,
                isFactured);

            var pagedDeliveryNote = await mediator.Send(query, cancellationToken);

            var metadata = new
            {
                pagedDeliveryNote.TotalCount,
                pagedDeliveryNote.PageSize,
                pagedDeliveryNote.CurrentPage,
                pagedDeliveryNote.TotalPages,
                pagedDeliveryNote.HasNext,
                pagedDeliveryNote.HasPrevious
            };

            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

            return Results.Ok(pagedDeliveryNote);
        });
    }
}