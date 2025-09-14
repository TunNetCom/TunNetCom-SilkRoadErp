using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
{
    public class GetProviderEndpointTest
    {
        [Fact]
        public async Task GetProviderEndpoint_ReturnsOk_WithPaginationMetadata()
        {
            // Arrange
            var paginationParams = new QueryStringParameters
            {
                PageNumber = 1,
                PageSize = 10,
                SearchKeyword = "test"
            };
            var pagedListMock = new PagedList<ProviderResponse>(
                new List<ProviderResponse> { new() { Id = 1, Nom = "Test Provider" } },
                count: 1,
                pageNumber: 1,
                pageSize: 10);
            var mediatorMock = new Mock<IMediator>();
            _ = mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedListMock);
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            var cancellationToken = CancellationToken.None;
            var handler = async (
                QueryStringParameters query,
                IMediator mediator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var q = new GetProviderQuery(query.PageNumber, query.PageSize, query.SearchKeyword);
                var pagedProviders = await mediator.Send(q, ct);
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
            };
            // Act
            var result = await handler(paginationParams, mediatorMock.Object, context, cancellationToken);
            // Assert
            _ = result.Should().BeOfType<Ok<PagedList<ProviderResponse>>>();
            _ = context.Response.Headers.Should().ContainKey("X-Pagination");
        }
    }
}
