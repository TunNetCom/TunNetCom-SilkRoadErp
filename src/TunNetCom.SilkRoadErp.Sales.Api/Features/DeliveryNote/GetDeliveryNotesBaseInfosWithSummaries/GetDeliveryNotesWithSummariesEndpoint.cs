using Microsoft.Data.SqlClient;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesWithSummaries;

public class GetDeliveryNotesWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/deliverynotes/summaries", async Task<Results<Ok<GetDeliveryNotesWithSummariesResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> (
            IMediator mediator,
            [AsParameters] GetDeliveryNotesQueryParams queryParams,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDeliveryNotesBaseInfosWithSummariesQuery(
                PageNumber: queryParams.PageNumber ?? 1,
                PageSize: queryParams.PageSize ?? 10,
                IsInvoiced: queryParams.IsInvoiced,
                SortOrder : queryParams.SortOrder,
                SortProperty: queryParams.SortProperty,
                CustomerId: queryParams.CustomerId,
                InvoiceId: queryParams.InvoiceId
            );

            var response = await mediator.Send(query, cancellationToken);

            if (!response.GetDeliveryNoteBaseInfos.Any())
            {
                return TypedResults.NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "No delivery notes found",
                    Detail = "No delivery notes found for the given criteria."
                });
            }

            return TypedResults.Ok(response);
        })
        .WithName("GetDeliveryNotesWithSummaries")
        .Produces<GetDeliveryNotesWithSummariesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
        .ProducesProblem(StatusCodes.Status404NotFound, "application/problem+json")
        .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
        .WithDescription("Gets a paginated list of delivery notes with summaries for a customer, optionally filtered by invoice ID, invoiced status, and search keyword.");
    }
}