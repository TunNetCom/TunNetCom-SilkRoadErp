using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.Api.Tests.Features.priceQuote.UpdatePriceQuote
{
    public class UpdatePriceQuoteEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        public UpdatePriceQuoteEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }
        [Fact]
        public async Task ShouldReturnNoContent_WhenUpdateSuccessful()
        {
            // Arrange
            var request = new UpdateQuotationRequest
            {
                IdClient = 1,
                Date = DateTime.UtcNow,
                TotHTva = 100,
                TotTva = 19,
                TotTtc = 119
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePriceQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Result.Ok());
            // Act
            var result = await InvokeEndpointAsync(123, request);
            // Assert
            Assert.IsType<NoContent>(result);
        }

        [Fact]
        public async Task ShouldReturnNotFound_WhenEntityNotFound()
        {
            var request = new UpdateQuotationRequest
            {
                IdClient = 2,
                Date = DateTime.UtcNow,
                TotHTva = 150,
                TotTva = 25,
                TotTtc = 175
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePriceQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Result.Fail(EntityNotFound.Error()));
            var result = await InvokeEndpointAsync(222, request);
            Assert.IsType<NotFound>(result);
        }

        [Fact]
        public async Task ShouldReturnValidationProblem_WhenValidationFails()
        {
            var request = new UpdateQuotationRequest
            {
                IdClient = 3,
                Date = DateTime.UtcNow,
                TotHTva = 200,
                TotTva = 38,
                TotTtc = 238
            };
            var error = new Error("quotation_num_exist");
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePriceQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Result.Fail(error));
            var result = await InvokeEndpointAsync(333, request);
            var problemResult = Assert.IsType<ValidationProblem>(result);
            Assert.Contains("errors", problemResult.ProblemDetails.Errors.Keys);
            Assert.Contains("quotation_num_exist", problemResult.ProblemDetails.Errors["errors"]);
        }

        private async Task<object> InvokeEndpointAsync(int num, UpdateQuotationRequest request)
        {
            var updatePriceQuoteCommand = new UpdatePriceQuoteCommand(
                Num: num,
                IdClient: request.IdClient,
                Date: request.Date,
                TotHTva: request.TotHTva,
                TotTva: request.TotTva,
                TotTtc: request.TotTtc);
            var result = await _mediatorMock.Object.Send(updatePriceQuoteCommand, CancellationToken.None);
            if (result.IsEntityNotFound())
                return TypedResults.NotFound();
            if (result.IsFailed)
                return result.ToValidationProblem();
            return TypedResults.NoContent();
        }
    }
}
