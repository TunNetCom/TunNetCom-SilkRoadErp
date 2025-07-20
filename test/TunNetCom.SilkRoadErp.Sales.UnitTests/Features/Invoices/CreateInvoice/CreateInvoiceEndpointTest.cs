using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.CreateInvoice
{
    public class CreateInvoiceEndpointTests
    {
        [Fact]
        public async Task HandleCreateInvoiceAsync_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var request = new CreateInvoiceRequest
            {
                Date = DateTime.Today,
                ClientId = 1
            };
            var expectedInvoiceNum = 123;
            mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(expectedInvoiceNum));
            var endpoint = new CreateInvoiceEndpoint();
            // Act
            var result = await endpoint.HandleCreateInvoiceAsync(mediatorMock.Object, request, CancellationToken.None);
            // Assert
            var createdResult = result.Result as Created<CreateInvoiceRequest>;
            createdResult.Should().NotBeNull();
            createdResult!.Location.Should().Be($"/invoices/{expectedInvoiceNum}");
            createdResult.Value.Should().Be(request);
        }

        [Fact]
        public async Task HandleCreateInvoiceAsync_ShouldReturnValidationProblem_WhenFailed()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var request = new CreateInvoiceRequest
            {
                Date = DateTime.Today,
                ClientId = 999 
            };
            var failedResult = Result.Fail<int>("not_found");
            mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);
            var endpoint = new CreateInvoiceEndpoint();
            // Act
            var result = await endpoint.HandleCreateInvoiceAsync(mediatorMock.Object, request, CancellationToken.None);
            // Assert
            var problemResult = result.Result as ValidationProblem;
            problemResult.Should().NotBeNull();
        }
    }
}
