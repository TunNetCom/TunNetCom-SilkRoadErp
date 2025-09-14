using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.GetFullInvoiceByIdTest
{
    public class GetFullInvoiceByIdEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;

        public GetFullInvoiceByIdEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task HandleGetFullInvoiceByIdAsync_ReturnsOk_WithValidData()
        {
            // Arrange
            var invoiceId = 123;
            var invoiceResponse = new FullInvoiceResponse
            {
                IdClient = invoiceId,
            };
            var expectedResult = Result.Ok(invoiceResponse);

            _ = _mediatorMock.Setup(m => m.Send(It.IsAny<GetFullInvoiceByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await GetFullInvoiceByIdEndpoint.HandleGetFullInvoiceByIdAsync(
                _mediatorMock.Object,
                invoiceId,
                CancellationToken.None);

            // Assert
            var typedResult = Assert.IsType<Ok<FullInvoiceResponse>>(result.Result);
            Assert.Equal(invoiceId, typedResult.Value.IdClient);
        }
        [Fact]
        public async Task HandleGetFullInvoiceByIdAsync_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var invoiceId = 456;
            var expectedResult = EntityNotFound.Error();

            _ = _mediatorMock.Setup(m => m.Send(It.IsAny<GetFullInvoiceByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await GetFullInvoiceByIdEndpoint.HandleGetFullInvoiceByIdAsync(
                _mediatorMock.Object,
                invoiceId,
                CancellationToken.None);

            // Assert
            _ = Assert.IsType<NotFound>(result.Result);
        }
    }
}
