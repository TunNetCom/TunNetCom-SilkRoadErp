using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.DeletePriceQuote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.PriceQuotes
{
    public class DeletePriceQuoteEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;

        public DeletePriceQuoteEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }
        private async Task<Results<NoContent, NotFound>> InvokeEndpoint(int num, Result handlerResult)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<DeletePriceQuoteCommand>(c => c.Num == num), It.IsAny<CancellationToken>()))
                .ReturnsAsync(handlerResult);
            var endpoint = new DeletePriceQuoteEndpoint();
            return await new Func<IMediator, int, CancellationToken, Task<Results<NoContent, NotFound>>>(
                async (mediator, num, cancellationToken) =>
                {
                    var deletePriceQuoteCommand = new DeletePriceQuoteCommand(num);
                    var deleteResult = await mediator.Send(deletePriceQuoteCommand, cancellationToken);
                    return deleteResult.IsFailed
                        ? TypedResults.NotFound()
                        : TypedResults.NoContent();
                })(_mediatorMock.Object, num, CancellationToken.None);
        }

        [Fact]
        public async Task Should_Return_NoContent_When_Delete_Is_Successful()
        {
            // Arrange
            var num = 123;
            var handlerResult = Result.Ok();
            // Act
            var result = await InvokeEndpoint(num, handlerResult);
            // Assert
            result.Result.Should().BeOfType<NoContent>();
        }

        [Fact]
        public async Task Should_Return_NotFound_When_Delete_Fails()
        {
            // Arrange
            var num = 456;
            var handlerResult = Result.Fail("not_found");
            // Act
            var result = await InvokeEndpoint(num, handlerResult);
            // Assert
            result.Result.Should().BeOfType<NotFound>();
        }
    }
}