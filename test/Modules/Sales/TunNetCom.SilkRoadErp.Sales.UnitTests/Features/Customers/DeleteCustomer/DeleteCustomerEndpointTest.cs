namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class DeleteCustomerEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DeleteCustomerEndpoint _endpoint;
        public DeleteCustomerEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new DeleteCustomerEndpoint();
        }
        [Fact]
        public async Task HandleDeleteCustomerAsync_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var result = await _endpoint.HandleDeleteCustomerAsync(_mediatorMock.Object, id, CancellationToken.None);
            // Assert
            _ = Assert.IsType<NoContent>(result.Result);
        }

        [Fact]
        public async Task HandleDeleteCustomerAsync_ShouldReturnValidationProblem_WhenFailureIsNotNotFound()
        {
            // Arrange
            int id = 3;
            var failedResult = Result.Fail("Validation failed");
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);
            // Act
            var result = await _endpoint.HandleDeleteCustomerAsync(_mediatorMock.Object, id, CancellationToken.None);
            // Assert
            _ = Assert.IsType<ValidationProblem>(result.Result);
        }
    }
}
