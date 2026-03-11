using Carter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.GetTiersDepenseFonctionnement;

public class GetTiersDepenseFonctionnementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tiers-depenses-fonctionnement", async (
            [AsParameters] QueryStringParameters paginationQueryParams,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTiersDepenseFonctionnementQuery(
                paginationQueryParams.PageNumber,
                paginationQueryParams.PageSize,
                paginationQueryParams.SearchKeyword);
            var paged = await mediator.Send(query, cancellationToken);
            var metadata = new
            {
                paged.TotalCount,
                paged.PageSize,
                paged.CurrentPage,
                paged.TotalPages,
                paged.HasNext,
                paged.HasPrevious
            };
            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);
            return Results.Ok(paged);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewTiersDepenseFonctionnement}")
        .WithTags(EndpointTags.TiersDepenseFonctionnement);
    }
}
