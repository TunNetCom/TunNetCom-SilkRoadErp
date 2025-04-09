namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails;
public class GetReceiptNoteWithDetailsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/receipt-note",
            async Task<Results<Ok<PagedList<ReceiptNoteDetailsResponse>>, BadRequest<ProblemDetails>>> (
            IMediator mediator,
            [AsParameters] GetReceiptNoteWithDetailsQuery queryParams,
            CancellationToken cancellationToken) =>
            {
                var query = new GetReceiptNoteWithDetailsQuery(
                    IdFournisseur: queryParams.IdFournisseur,
                    PageNumber: queryParams.PageNumber ,
                    PageSize: queryParams.PageSize,
                    SearchKeyword: queryParams.SearchKeyword,
                    IsInvoiced: queryParams.IsInvoiced
                );
                var response = await mediator.Send(query, cancellationToken);

                return TypedResults.Ok(response);
            })
        .WithName("GetReceiptNote")
        .Produces<PagedList<ReceiptNoteDetailsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
        .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
        .WithDescription("Gets a paginated list of receipt note,  receipt note status, and search keyword.");
    }
}