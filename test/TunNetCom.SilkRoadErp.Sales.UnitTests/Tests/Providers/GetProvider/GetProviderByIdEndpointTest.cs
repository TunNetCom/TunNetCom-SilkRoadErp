using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
{
    public class GetProviderByIdEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock = new();
        [Fact]
        public async Task GetProviderById_ReturnsOk_WhenFound()
        {
            // Arrange
            var providerId = 1;
            var provider = new ProviderResponse { Id = providerId, Nom = "Test Provider" };
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderByIdQuery>(q => q.Id == providerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(provider));
            var handler = GetHandler();
            // Act
            var result = await handler(_mediatorMock.Object, providerId, CancellationToken.None);
            // Assert
            var typedResult = result as Results<Ok<ProviderResponse>, NotFound>;
            typedResult.Should().NotBeNull();
            var actualResult = typedResult.Result;
            actualResult.Should().BeOfType<Ok<ProviderResponse>>();
            var okResult = (Ok<ProviderResponse>)actualResult;
            okResult.Value.Should().BeEquivalentTo(provider);
        }
        [Fact]
        public async Task GetProviderById_ReturnsNotFound_WhenEntityNotFound()
        {
            // Arrange
            var providerId = 999;
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderByIdQuery>(q => q.Id == providerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<ProviderResponse>(EntityNotFound.Error()));
            var handler = GetHandler();
           // Act
            var result = await handler(_mediatorMock.Object, providerId, CancellationToken.None);
            // Assert
            var typedResult = result as Results<Ok<ProviderResponse>, NotFound>;
            typedResult.Should().NotBeNull();
            var actualResult = typedResult.Result;
            actualResult.Should().BeOfType<NotFound>();
        }

        private static Func<IMediator, int, CancellationToken, Task<Results<Ok<ProviderResponse>, NotFound>>> GetHandler()
        {
            return async (mediator, id, ct) =>
            {
                var query = new GetProviderByIdQuery(id);
                var result = await mediator.Send(query, ct);
                if (result.IsFailed)
                {
                    if (result.HasError<EntityNotFound>())
                    {
                        return TypedResults.NotFound();
                    }
                    return TypedResults.NotFound(); // Fallback for any failure
                }
                return TypedResults.Ok(result.Value);
            };
        }
    }
}