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
            var endpoint = async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var query = new GetOrdersListQuery();
                var result = await mediator.Send(query, cancellationToken);

                if (result.IsFailed)
                {
                    return Results.BadRequest(new { Message = "An error occurred", Errors = result.Errors });
                }
                return Results.Ok(result.Value);
            };
            // Act
            var result = await endpoint(mockMediator.Object, CancellationToken.None);
            // Assert
            var okResult = Assert.IsType<Ok<List<OrderSummaryResponse>>>(result);
            Assert.Single(okResult.Value);
            Assert.Equal(1, okResult.Value[0].OrderNumber);
        }
        [Fact]
        public async Task GetOrdersList_ReturnsBadRequest_WhenMediatorFails()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var errors = new List<IError> { EntityNotFound.Error() };

            mockMediator.Setup(m => m.Send(It.IsAny<GetOrdersListQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Fail<List<OrderSummaryResponse>>(errors));

            var endpoint = async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var query = new GetOrdersListQuery();
                var result = await mediator.Send(query, cancellationToken);

                if (result.IsFailed)
                {
                    return Results.BadRequest(new { Message = "An error occurred", Errors = result.Errors });
                }

                return Results.Ok(result.Value);
            };

            // Act
            var result = await endpoint(mockMediator.Object, CancellationToken.None);
            // Assert : vérifie que c’est bien un BadRequest
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IResult>(result);
            Assert.StartsWith("BadRequest", result.GetType().Name);
            var valueProperty = result.GetType().GetProperty("Value");
            Assert.NotNull(valueProperty);
            var value = valueProperty!.GetValue(result);
            Assert.NotNull(value);
            var messageProperty = value.GetType().GetProperty("Message");
            Assert.NotNull(messageProperty);
            var messageValue = messageProperty!.GetValue(value)?.ToString();
            Assert.Equal("An error occurred", messageValue);
        }
    }
}