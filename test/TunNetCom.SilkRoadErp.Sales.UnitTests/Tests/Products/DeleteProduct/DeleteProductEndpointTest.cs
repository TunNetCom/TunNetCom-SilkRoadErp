namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products
{
    public class DeleteProductEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Func<IMediator, string, CancellationToken, Task<Results<NoContent, NotFound>>> _handler;
        public DeleteProductEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = async (mediator, refe, ct) =>
            {
                var deleteCommand = new DeleteProductCommand(refe);
                var deleteResult = await mediator.Send(deleteCommand, ct);
                if (deleteResult.IsFailed && deleteResult.Errors.Any(e => e.Message == "not_found"))
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.NoContent();
            };
        }

        [Fact]
        public async Task Endpoint_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productRef = "nonexistent";
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail("not_found")); // Simule produit non trouvé
            // Act
            var response = await _handler(_mediatorMock.Object, productRef, CancellationToken.None);
            // Assert
            var notFoundResult = Assert.IsType<NotFound>(response.Result);
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Endpoint_ReturnsNoContent_WhenProductDeleted()
        {
            // Arrange
            var productRef = "toDelete";
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var response = await _handler(_mediatorMock.Object, productRef, CancellationToken.None);
            // Assert
            var noContentResult = Assert.IsType<NoContent>(response.Result);
            noContentResult.Should().NotBeNull();
        }
    }
}