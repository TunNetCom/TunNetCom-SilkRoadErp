//using Newtonsoft.Json;
//using TunNetCom.SilkRoadErp.Sales.Contracts;
//using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

//namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.Endpoints;

//public class GetCustomersEndpointTests
//{
//    private readonly Mock<IMediator> _mediatorMock;
//    private readonly DefaultHttpContext _httpContext;

//    public GetCustomersEndpointTests()
//    {
//        _mediatorMock = new Mock<IMediator>();
//        _httpContext = new DefaultHttpContext();
//    }

//    [Fact]
//    public async Task HandleGetCustomersAsync_ReturnsOk_WithValidData()
//    {
//        // Arrange
//        var paginationParams = new QueryStringParameters { PageNumber = 1, PageSize = 10, SearchKeyword = "John" };
//        var pagedCustomers = new PagedList<CustomerResponse>(
//            new List<CustomerResponse> { new() { Id = 1, Nom = "John Doe" } },
//            count: 1, pageNumber: 1, pageSize: 10);

//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerQuery>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(pagedCustomers);

//        // Act
//        var result = await GetCustomersEndpoint.HandleGetCustomersAsync(paginationParams, _mediatorMock.Object, _httpContext, CancellationToken.None);

//        // Assert
//        var typedResult = Assert.IsType<Ok<PagedList<CustomerResponse>>>(result.Result);
//        Assert.Equal(1, typedResult.Value.TotalCount);
//        Assert.Equal(10, typedResult.Value.PageSize);
//        Assert.Single(typedResult.Value);
//        Assert.Equal("John Doe", typedResult.Value[0].Nom);

//        var paginationHeader = _httpContext.Response.Headers["X-Pagination"].ToString();
//        Assert.False(string.IsNullOrEmpty(paginationHeader));

//        var metadata = JsonConvert.DeserializeObject<dynamic>(paginationHeader);
//        Assert.Equal(1, (int)metadata.TotalCount);
//        Assert.Equal(10, (int)metadata.PageSize);
//    }
//}
