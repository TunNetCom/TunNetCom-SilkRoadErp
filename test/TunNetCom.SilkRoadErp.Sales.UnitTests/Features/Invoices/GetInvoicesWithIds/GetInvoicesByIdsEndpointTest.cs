using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithIds;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.GetInvoicesWithIdsTest
{
    public class GetInvoicesByIdsEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        public GetInvoicesByIdsEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }
        [Fact]
        public async Task HandleGetInvoicesByIdsAsync_ReturnsOk_WithValidInvoices()
        {
            // Arrange
            var invoicesIds = new List<int> { 1, 2 };
            var invoices = new List<InvoiceResponse>
            {
                new InvoiceResponse { Number = 1, CustomerId = 10, TotalExcludingTaxAmount = 100, TotalVATAmount = 19, TotalIncludingTaxAmount = 119 },
                new InvoiceResponse { Number = 2, CustomerId = 11, TotalExcludingTaxAmount = 200, TotalVATAmount = 38, TotalIncludingTaxAmount = 238 },
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInvoicesWithIdsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Result.Ok(invoices));
            // Act
            var result = await GetInvoicesByIdsEndpoint.HandleGetInvoicesByIdsAsync(
                _mediatorMock.Object,
                invoicesIds,
                CancellationToken.None);
            // Assert
            var resultUnion = Assert.IsType<Results<Ok<List<InvoiceResponse>>, BadRequest<List<IError>>>>(result);
            var okResult = Assert.IsType<Ok<List<InvoiceResponse>>>(result.Result);
            Assert.Equal(2, okResult.Value.Count);
            Assert.Equal(1, okResult.Value[0].Number);
        }

        [Fact]
        public async Task HandleGetInvoicesByIdsAsync_ReturnsOk_WithEmptyList_WhenNoInvoicesFound()
        {
            // Arrange
            var invoicesIds = new List<int> { 1000 };
            var emptyResult = new List<InvoiceResponse>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInvoicesWithIdsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Result.Ok(emptyResult));
            // Act
            var result = await GetInvoicesByIdsEndpoint.HandleGetInvoicesByIdsAsync(
                _mediatorMock.Object,
                invoicesIds,
                CancellationToken.None);
            // Assert
            var resultUnion = Assert.IsType<Results<Ok<List<InvoiceResponse>>, BadRequest<List<IError>>>>(result);
            var okResult = Assert.IsType<Ok<List<InvoiceResponse>>>(result.Result);
            Assert.Empty(okResult.Value);
        }

        [Fact]
        public async Task HandleGetInvoicesByIdsAsync_ReturnsBadRequest_WhenResultIsFailed()
        {
            // Arrange
            var invoicesIds = new List<int> { 123 };
            var failResult = Result.Fail<List<InvoiceResponse>>("Unexpected error");
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInvoicesWithIdsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(failResult);
            // Act
            var result = await GetInvoicesByIdsEndpoint.HandleGetInvoicesByIdsAsync(
                _mediatorMock.Object,
                invoicesIds,
                CancellationToken.None);
            // Assert
            var resultUnion = Assert.IsType<Results<Ok<List<InvoiceResponse>>, BadRequest<List<IError>>>>(result);
            var badRequestResult = Assert.IsType<BadRequest<List<IError>>>(result.Result);
            Assert.Single(badRequestResult.Value);
            Assert.Equal("Unexpected error", badRequestResult.Value[0].Message);
        }
    }

    public static class GetInvoicesByIdsEndpoint
    {
        public static async Task<Results<Ok<List<InvoiceResponse>>, BadRequest<List<IError>>>> HandleGetInvoicesByIdsAsync(
            IMediator mediator,
            List<int> invoicesIds,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetInvoicesWithIdsQuery(invoicesIds), cancellationToken);
            if (result.IsFailed)
                return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
    }
}
