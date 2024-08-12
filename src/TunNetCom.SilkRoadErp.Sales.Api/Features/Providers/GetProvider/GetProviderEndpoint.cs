namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProvider;
    public class GetProviderEndpoint : ICarterModule
    {
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/providers", async (
            [AsParameters] QueryStringParameters paginationQueryParams,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProviderQuery(
                paginationQueryParams.PageNumber,
                paginationQueryParams.PageSize,
                paginationQueryParams.SearchKeyword);

            var pagedProviders = await mediator.Send(query, cancellationToken);

            var metadata = new
            {
                pagedProviders.TotalCount,
                pagedProviders.PageSize,
                pagedProviders.CurrentPage,
                pagedProviders.TotalPages,
                pagedProviders.HasNext,
                pagedProviders.HasPrevious
            };

            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

            return Results.Ok(pagedProviders);
        });
    }
}
