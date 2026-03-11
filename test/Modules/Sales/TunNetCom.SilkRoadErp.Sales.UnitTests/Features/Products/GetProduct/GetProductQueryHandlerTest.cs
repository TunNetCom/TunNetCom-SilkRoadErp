using Newtonsoft.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
public class GetProductEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetProductEndpoint _endpoint;
    public GetProductEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new GetProductEndpoint();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithPagedProductsAndAddPaginationHeader()
    {
        // Arrange
        var paginationParams = new QueryStringParameters
        {
            PageNumber = 2,
            PageSize = 5,
            SearchKeyword = "test",
            SortProprety = "Nom",
            SortOrder = "asc"
        };
        var pagedList = new PagedList<ProductResponse>(
            items: new List<ProductResponse>
            {
                new() { Reference = "P001", Name = "Produit 1" },
                new() { Reference = "P002", Name = "Produit 2" }
            },
            count: 10,
            pageNumber: 2,
            pageSize: 5
            );
        _ = _mediatorMock
            .Setup(m => m.Send(It.Is<GetProductQuery>(q =>
                q.PageNumber == paginationParams.PageNumber &&
                q.PageSize == paginationParams.PageSize &&
                q.SearchKeyword == paginationParams.SearchKeyword &&
                q.SortProprety == paginationParams.SortProprety &&
                q.SortOrder == paginationParams.SortOrder),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedList);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Headers.Clear();
        // Act
        var result = await _endpoint.Handle(
            paginationParams,
            _mediatorMock.Object,
            httpContext,
            CancellationToken.None);
        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<PagedList<ProductResponse>>>(result);
        _ = okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        _ = okResult.Value.Should().BeEquivalentTo(pagedList);      
        Assert.True(httpContext.Response.Headers.ContainsKey("X-Pagination"));
        var headerValue = httpContext.Response.Headers["X-Pagination"];
        var metadata = JsonConvert.DeserializeObject<dynamic>(headerValue);
        Assert.Equal(pagedList.TotalCount, (int)metadata.TotalCount);
        Assert.Equal(pagedList.PageSize, (int)metadata.PageSize);
        Assert.Equal(pagedList.CurrentPage, (int)metadata.CurrentPage);
        Assert.Equal(pagedList.TotalPages, (int)metadata.TotalPages);
        Assert.Equal(pagedList.HasNext, (bool)metadata.HasNext);
        Assert.Equal(pagedList.HasPrevious, (bool)metadata.HasPrevious);
    }
}
