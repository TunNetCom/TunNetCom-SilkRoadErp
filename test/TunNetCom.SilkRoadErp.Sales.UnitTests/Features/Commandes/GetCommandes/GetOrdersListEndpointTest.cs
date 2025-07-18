using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Commandes.GetCommandes
{
    public class GetOrdersListEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        public GetOrdersListEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }
        public record ErrorResponse(string Message, List<IError> Errors);
        private static async Task<IResult> CallHandlerAsync(IMediator mediator, CancellationToken cancellationToken)
        {
            var query = new GetOrdersListQuery();
            var result = await mediator.Send(query, cancellationToken);
            if (result.IsFailed)
            {
                return Results.BadRequest(new ErrorResponse("An error occurred while retrieving the orders list.", result.Errors));
            }
            return Results.Ok(result.Value);
        }

        [Fact]
        public async Task GetOrdersList_Success_ReturnsOk()
        {
            // Arrange
            var orders = new List<OrderSummaryResponse>
            {
                new OrderSummaryResponse
                {
                    OrderNumber = 100,
                    SupplierId = 1,
                    Date = System.DateTime.Today,
                    TotalExcludingVat = 100,
                    TotalVat = 20,
                    NetToPay = 120
                }
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetOrdersListQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(orders));
            // Act
            var result = await CallHandlerAsync(_mediatorMock.Object, CancellationToken.None);
           // Assert
            var okResult = result as Ok<List<OrderSummaryResponse>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(orders);
        }
        [Fact]
        public async Task GetOrdersList_Failure_ReturnsBadRequest()
        {
            // Arrange
            var errorMessage = "Database error";
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetOrdersListQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<List<OrderSummaryResponse>>(errorMessage));
            // Act
            var result = await CallHandlerAsync(_mediatorMock.Object, CancellationToken.None);
            // Assert
            result.Should().BeOfType<BadRequest<ErrorResponse>>();

            var badRequest = result as BadRequest<ErrorResponse>;
            badRequest.Should().NotBeNull();
            badRequest!.Value.Message.Should().Contain("error");
            badRequest.Value.Errors.Should().Contain(e => e.Message == errorMessage);
        }

        [Fact]
        public async Task GetOrdersList_CancellationRequested_ThrowsOperationCanceledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetOrdersListQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                CallHandlerAsync(_mediatorMock.Object, cts.Token));
        }
    }
}
