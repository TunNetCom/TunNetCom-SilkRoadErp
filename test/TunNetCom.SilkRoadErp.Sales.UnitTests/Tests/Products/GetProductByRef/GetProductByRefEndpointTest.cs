using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products
{
    public class GetProductByRefEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Func<IMediator, string, CancellationToken, Task<Results<Ok<ProductResponse>, NotFound>>> _handler;
        public GetProductByRefEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = async (mediator, refe, ct) =>
            {
                var query = new GetProductByRefQuery(refe);
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

        [Fact]
        public async Task Endpoint_ReturnsOk_WhenProductExists()
        {
            // Arrange
            var refe = "existingRef";
            var productResponse = new ProductResponse
            {
                Reference = refe,
                Name = "Product Test",
                Qte = 10,
                QteLimit = 5,
                DiscountPourcentage = 1.5,
                DiscountPourcentageOfPurchasing = 0.5,
                VatRate = 19,
                Price = 100,
                PurchasingPrice = 80,
                Visibility = true
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProductByRefQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(productResponse));
            // Act
            var response = await _handler(_mediatorMock.Object, refe, CancellationToken.None);
            // Assert
            var okResult = response.Result.Should().BeOfType<Ok<ProductResponse>>().Subject;
            _ = okResult.Value.Should().BeEquivalentTo(productResponse);
        }

        [Fact]
        public async Task Endpoint_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productRef = "nonexistent";
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProductByRefQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<ProductResponse>(EntityNotFound.Error()));
            // Act
            var response = await _handler(_mediatorMock.Object, productRef, CancellationToken.None);
            // Assert
            _ = response.Result.Should().BeOfType<NotFound>();
        }
    }
}