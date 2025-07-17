using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Commande
{
    public class GetOrdersListEndpointTests
    {
        [Fact]
        public async Task GetOrdersList_ReturnsOk_WithOrders()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var orders = new List<OrderSummaryResponse>
            {
                new OrderSummaryResponse { OrderNumber = 1, SupplierId = 10, TotalExcludingVat = 100, NetToPay = 120, TotalVat = 20 }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<GetOrdersListQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result.Ok(orders));

            // Act
            var result = await GetOrdersListEndpoint.HandleAsync(mockMediator.Object, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<Ok<List<OrderSummaryResponse>>>(result);
            var returnedOrders = okResult.Value;
            Assert.NotNull(returnedOrders);
            Assert.Single(returnedOrders);
            Assert.Equal(1, returnedOrders[0].OrderNumber);
        }

        [Fact]
        public async Task GetOrdersList_ReturnsBadRequest_WhenMediatorFails()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var errors = new List<IError> { EntityNotFound.Error() };

            mockMediator.Setup(m => m.Send(It.IsAny<GetOrdersListQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Fail<List<OrderSummaryResponse>>(errors));

            // Act
            var result = await GetOrdersListEndpoint.HandleAsync(mockMediator.Object, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequest<ErrorResponse>>(result);
            var value = badRequest.Value;

            Assert.NotNull(value);
            Assert.Equal("An error occurred", value.Message);
            Assert.NotNull(value.Errors);
            Assert.Single(value.Errors);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<IError> Errors { get; set; } = new();
    }

    public static class GetOrdersListEndpoint
    {
        public static async Task<IResult> HandleAsync(IMediator mediator, CancellationToken cancellationToken)
        {
            var query = new GetOrdersListQuery();
            var result = await mediator.Send(query, cancellationToken);

            return result.IsFailed
                ? Results.BadRequest(new ErrorResponse
                {
                    Message = "An error occurred",
                    Errors = result.Errors
                })
                : Results.Ok(result.Value);
        }
    }
}
