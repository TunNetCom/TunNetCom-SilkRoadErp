//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Routing;
//using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById;
//using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
//namespace TunNetCom.SilkRoadErp.Sales.Api.Tests.Features.priceQuote.GetPriceQuoteById
//{
//    public class GetPriceQuoteByIdEndpointTest
//    {
//        private readonly Mock<IMediator> _mediatorMock = new();
//        private readonly GetPriceQuoteByIdEnpoint _endpoint = new();

//        [Fact]
//        public async Task HandleAsync_WithValidId_ReturnsOkWithQuotation()
//        {
//            // Arrange
//            var quotationId = 123;
//            var expectedResponse = new QuotationResponse
//            {
//                Num = quotationId,
//                IdClient = 456,
//                Date = DateTime.UtcNow,
//                TotHTva = 1000,
//                TotTva = 200,
//                TotTtc = 1200
//            };
//            _ = _mediatorMock.Setup(m => m.Send(It.Is<GetPriceQuoteByIdQuery>(q => q.Num == quotationId), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(Result.Ok(expectedResponse));
//            // Act
//            var result = await _endpoint.HandleRequest(_mediatorMock.Object, quotationId, CancellationToken.None);
//            // Assert
//            var okResult = Assert.IsType<Ok<QuotationResponse>>(result);
//            Assert.Equal(expectedResponse, okResult.Value);
//        }

//        [Fact]
//        public async Task HandleAsync_WithNonExistentId_ReturnsNotFound()
//        {
//            // Arrange
//            var quotationId = 999;
//            var error = new Error("Not found");
//            error.Metadata.Add("IsEntityNotFound", true);
//            _ = _mediatorMock.Setup(m => m.Send(It.Is<GetPriceQuoteByIdQuery>(q => q.Num == quotationId), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(Result.Fail<QuotationResponse>(error));
//            // Act
//            var result = await _endpoint.HandleRequest(_mediatorMock.Object, quotationId, CancellationToken.None);
//            // Assert
//            _ = Assert.IsType<NotFound>(result);
//        }

//        public class TestEndpointRouteBuilder : IEndpointRouteBuilder
//        {
//            private readonly List<EndpointDataSource> _dataSources = new();
//            public TestEndpointRouteBuilder()
//            {
//                ServiceProvider = new Mock<IServiceProvider>().Object;
//            }
//            public IServiceProvider ServiceProvider { get; }
//            public ICollection<EndpointDataSource> DataSources => _dataSources;
//            public IApplicationBuilder CreateApplicationBuilder() => new Mock<IApplicationBuilder>().Object;
//        }
//    }
//    public static class GetPriceQuoteByIdEnpointExtensions
//    {
//        public static async Task<IResult> HandleRequest(
//            this GetPriceQuoteByIdEnpoint endpoint,
//            IMediator mediator,
//            int num,
//            CancellationToken cancellationToken)
//        {
//            var query = new GetPriceQuoteByIdQuery(num);
//            var result = await mediator.Send(query, cancellationToken);
//            if (result.IsFailed && result.Errors.Any(e => e.Metadata.ContainsKey("IsEntityNotFound")))
//            {
//                return TypedResults.NotFound();
//            }
//            return TypedResults.Ok(result.Value);
//        }
//    }
//}