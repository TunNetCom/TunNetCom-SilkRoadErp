using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.Endpoints;

public class GetCustomerByIdEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;

    public GetCustomerByIdEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task HandleGetCustomerByIdAsync_ReturnsOk_WithValidData()
    {
        // Arrange
        var customerId = 1;
        var customerResponse = new CustomerResponse { Id = customerId, Name = "John Doe" };
        var expectedResult = Result.Ok(customerResponse);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await GetCustomerByIdEndpoint.HandleGetCustomerByIdAsync(
            _mediatorMock.Object,
            customerId,
            CancellationToken.None);

        // Assert
        var typedResult = Assert.IsType<Ok<CustomerResponse>>(result.Result);
        Assert.Equal(customerId, typedResult.Value.Id);
        Assert.Equal("John Doe", typedResult.Value.Name);
    }

    [Fact]
    public async Task HandleGetCustomerByIdAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = 1;
        var expectedResult = EntityNotFound.Error();

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await GetCustomerByIdEndpoint.HandleGetCustomerByIdAsync(
            _mediatorMock.Object,
            customerId,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }
}
