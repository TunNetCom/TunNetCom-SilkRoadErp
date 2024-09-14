using TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProvider;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNote;

public class GetReceiptNoteEndpoint : ICarterModule
{ public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/receiptnotes", async (
            [AsParameters] QueryStringParameters paginationQueryParams,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) => {
                var query = new GetReceiptNoteQuery(
                    paginationQueryParams.PageNumber,
                    paginationQueryParams.PageSize,
                    paginationQueryParams.SearchKeyword);

                var pagedReceipts = await mediator.Send(query, cancellationToken);

                var metadata = new
                {
                    pagedReceipts.TotalCount,
                    pagedReceipts.PageSize,
                    pagedReceipts.CurrentPage,
                    pagedReceipts.TotalPages,
                    pagedReceipts.HasNext,
                    pagedReceipts.HasPrevious
                };

                httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

                return Results.Ok(pagedReceipts);
            });
    }
}
