using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.Endpoints;

public class DeleteCustomerEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteCustomerEndpoint _endpoint;

    public DeleteCustomerEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new DeleteCustomerEndpoint();
    }

    [Fact]
    public async Task HandleDeleteCustomerAsync_ReturnsNoContent_WhenDeleteIsSuccessful()
    {
        // Arrange
        int customerId = 1;
        _ = _mediatorMock.Setup(
            m => m.Send(It.Is<DeleteCustomerCommand>(c => c.Id == customerId), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Result.Ok());

        // Act
        var result = await _endpoint.HandleDeleteCustomerAsync(_mediatorMock.Object, customerId, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsType<Results<NoContent, ValidationProblem, NotFound>>(result);
        _ = Assert.IsType<NoContent>(typedResult.Result);
    }

    [Fact]
    public async Task HandleDeleteCustomerAsync_ReturnsNotFound_WhenEntityNotFound()
    {
        // Arrange
        int customerId = 1;
        _ = _mediatorMock.Setup(
            m => m.Send(It.Is<DeleteCustomerCommand>(c => c.Id == customerId), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Result.Fail(EntityNotFound.Error()));

        // Act
        var result = await _endpoint
            .HandleDeleteCustomerAsync(_mediatorMock.Object, customerId, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsType<Results<NoContent, ValidationProblem, NotFound>>(result);
        _ = Assert.IsType<NotFound>(typedResult.Result);
    }

    [Fact]
    public async Task HandleDeleteCustomerAsync_ReturnsValidationProblem_WhenDeleteFails()
    {
        // Arrange
        int customerId = 1;
        _ = _mediatorMock.Setup(
            m => m.Send(It.Is<DeleteCustomerCommand>(c => c.Id == customerId), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Result.Fail(""));

        // Act
        var result = await _endpoint
            .HandleDeleteCustomerAsync(_mediatorMock.Object, customerId, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsType<Results<NoContent, ValidationProblem, NotFound>>(result);
        _ = Assert.IsType<ValidationProblem>(typedResult.Result);
    }

}
