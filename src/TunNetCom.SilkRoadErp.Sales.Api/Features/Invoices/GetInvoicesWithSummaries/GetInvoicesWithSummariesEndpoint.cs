using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithSummaries;

public class GetInvoicesWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/invoices/summaries",
            async Task<Results<Ok<GetInvoicesWithSummariesResponse>, BadRequest<ProblemDetails>>> (
                IMediator mediator,
                [AsParameters] GetInvoicesQueryParams queryParams,
                CancellationToken cancellationToken) =>
            {
                var query = new GetInvoicesWithSummariesQuery(
                    PageNumber: queryParams.PageNumber ?? 1,
                    PageSize: queryParams.PageSize ?? 10,
                    CustomerId: queryParams.CustomerId,
                    SortOrder: queryParams.SortOrder,
                    SortProperty: queryParams.SortProperty,
                    SearchKeyword: queryParams.SearchKeyword,
                    StartDate: queryParams.StartDate,
                    EndDate: queryParams.EndDate
                );

                var response = await mediator.Send(query, cancellationToken);

                return TypedResults.Ok(response);
            })
            .WithName("GetInvoicesWithSummaries")
            .WithTags(EndpointTags.Invoices)
            .Produces<GetInvoicesWithSummariesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Gets a paginated list of invoices with summaries, optionally filtered by customer ID, date range, and search keyword.");
    }
}

