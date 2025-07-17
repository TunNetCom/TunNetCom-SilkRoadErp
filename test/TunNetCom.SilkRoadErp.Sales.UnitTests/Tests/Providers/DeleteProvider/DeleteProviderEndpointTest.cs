using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
{
    public class DeleteProviderEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DeleteProviderEndpoint _endpoint;
        public DeleteProviderEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new DeleteProviderEndpoint();
        }
        [Fact]
        public async Task DeleteProvider_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int providerId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteProviderCommand>(c => c.Id == providerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            var handler = GetEndpointHandler();
            // Act
            var result = await handler(_mediatorMock.Object, providerId, CancellationToken.None);
            // Assert
            result.Result.Should().BeOfType<NoContent>();
        }

        [Fact]
        public async Task DeleteProvider_ReturnsNotFound_WhenEntityNotFound()
        {
            // Arrange
            int providerId = 999;
            var resultWithError = Result.Fail(EntityNotFound.Error());
            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteProviderCommand>(c => c.Id == providerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultWithError);
            var handler = GetEndpointHandler();
            // Act
            var result = await handler(_mediatorMock.Object, providerId, CancellationToken.None);
            // Assert
            var typedResult = result as Results<NoContent, NotFound>;
            typedResult.Should().NotBeNull();
            // Then check the actual result type
            var actualResult = typedResult.Result;
            actualResult.Should().BeOfType<NotFound>();
        }
        private static Func<IMediator, int, CancellationToken, Task<Results<NoContent, NotFound>>> GetEndpointHandler()
        {
            return async (mediator, id, cancellationToken) =>
            {
                var command = new DeleteProviderCommand(id);
                var result = await mediator.Send(command, cancellationToken);

                if (result.IsEntityNotFound())
                    return TypedResults.NotFound();

                return TypedResults.NoContent();
            };
        }
    }
}