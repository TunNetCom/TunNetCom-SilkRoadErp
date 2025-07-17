﻿namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProduct;
public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", Handle);
    }
    public async Task<IResult> Handle(
        [AsParameters] QueryStringParameters paginationQueryParams,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetProductQuery(
            paginationQueryParams.PageNumber,
            paginationQueryParams.PageSize,
            paginationQueryParams.SearchKeyword,
            paginationQueryParams.SortProprety,
            paginationQueryParams.SortOrder);

        var pagedProducts = await mediator.Send(query, cancellationToken);
        var metadata = new
        {
            pagedProducts.TotalCount,
            pagedProducts.PageSize,
            pagedProducts.CurrentPage,
            pagedProducts.TotalPages,
            pagedProducts.HasNext,
            pagedProducts.HasPrevious
        };
        httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);
        return Results.Ok(pagedProducts);
    }
}
