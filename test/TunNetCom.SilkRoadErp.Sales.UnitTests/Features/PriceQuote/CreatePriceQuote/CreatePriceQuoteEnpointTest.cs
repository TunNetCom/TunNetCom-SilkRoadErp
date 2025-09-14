using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Quotations
{
    public class CreatePriceQuoteEndpointTest
    {
        [Fact]
        public async Task CreateQuotation_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var request = new CreateQuotationRequest
            {
                Num = 3000,
                IdClient = 5,
                Date = DateTime.UtcNow,
                TotHTva = 100,
                TotTva = 19,
                TotTtc = 119
            };
            _ = mediatorMock.Setup(m => m.Send(It.IsAny<CreatePriceQuoteCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result.Ok(3000));
            // Act
            var result = await CreatePriceQuoteEnpointExtensions.CreateQuotationDelegate()
                .Invoke(mediatorMock.Object, request, CancellationToken.None);
            // Assert
            var createdResult = Assert.IsType<Created<CreateQuotationRequest>>(result.Result);
            Assert.Equal($"/quotations/{request.Num}", createdResult.Location);
            Assert.Equal(request, createdResult.Value);
        }

        [Fact]
        public async Task CreateQuotation_ShouldReturnValidationProblem_WhenFailed()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var request = new CreateQuotationRequest
            {
                Num = 3001,
                IdClient = 6,
                Date = DateTime.UtcNow,
                TotHTva = 100,
                TotTva = 19,
                TotTtc = 119
            };
            var resultFail = Result.Fail<int>("quotations_num_exist");
            _ = mediatorMock.Setup(m => m.Send(It.IsAny<CreatePriceQuoteCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(resultFail);
            // Act
            var result = await CreatePriceQuoteEnpointExtensions.CreateQuotationDelegate()
                .Invoke(mediatorMock.Object, request, CancellationToken.None);
            // Assert
            var problem = Assert.IsType<ValidationProblem>(result.Result);
            var details = Assert.IsType<HttpValidationProblemDetails>(problem.ProblemDetails);
            Assert.Contains(details.Errors.Values, errors => errors.Contains("quotations_num_exist"));
        }
    }

    internal static class CreatePriceQuoteEnpointExtensions
    {
        public static Func<IMediator, CreateQuotationRequest, CancellationToken, Task<Results<Created<CreateQuotationRequest>, ValidationProblem>>> CreateQuotationDelegate()
        {
            return async (mediator, request, cancellationToken) =>
            {
                var createPriceQuoteCommand = new CreatePriceQuoteCommand(
                    Num: request.Num,
                    IdClient: request.IdClient,
                    Date: request.Date,
                    TotHTva: request.TotHTva,
                    TotTva: request.TotTva,
                    TotTtc: request.TotTtc
                );
                var result = await mediator.Send(createPriceQuoteCommand, cancellationToken);
                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }
                return TypedResults.Created($"/quotations/{result.Value}", request);
            };
        }
    }
}
