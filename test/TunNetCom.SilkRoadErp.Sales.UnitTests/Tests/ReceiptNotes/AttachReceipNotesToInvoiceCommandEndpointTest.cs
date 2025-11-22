using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class AttachReceiptNotesToInvoiceEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AttachReceiptNotesToInvoiceEndpoint _endpoint;
        public AttachReceiptNotesToInvoiceEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new AttachReceiptNotesToInvoiceEndpoint();
        }
        private static async Task<object> InvokeEndpoint(
            AttachReceiptNotesToInvoiceCommand command,
            Mock<IMediator> mediatorMock)
        {
            var result = await mediatorMock.Object.Send(command, CancellationToken.None);

            if (result.HasError<EntityNotFound>())
                return TypedResults.NotFound();

            if (result.IsFailed)
                return result.ToValidationProblem();

            return TypedResults.NoContent();
        }
        [Fact]
        public async Task Handle_ValidCommand_ReturnsNoContent()
        {
            // Arrange
            var command = new AttachReceiptNotesToInvoiceCommand(new List<int> { 1, 2 }, 100);
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<AttachReceiptNotesToInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var result = await InvokeEndpoint(command, _mediatorMock);
            // Assert
            _ = Assert.IsType<NoContent>(result);
        }
        [Fact]
        public async Task Handle_EntityNotFound_ReturnsNotFound()
        {
            // Arrange
            var command = new AttachReceiptNotesToInvoiceCommand(new List<int> { 99 }, 999);

            // Ici on utilise une erreur typée EntityNotFound au lieu d'une string
            var failedResult = Result.Fail(EntityNotFound.Error("Invoice or receipt notes not found"));

            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<AttachReceiptNotesToInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);

            // Act
            var result = await InvokeEndpoint(command, _mediatorMock);

            // Assert
            _ = Assert.IsType<NotFound>(result);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsValidationProblem()
        {
            // Arrange
            var command = new AttachReceiptNotesToInvoiceCommand(new List<int>(), 0);
            var failedResult = Result.Fail("Validation error");
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<AttachReceiptNotesToInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);
            // Act
            var result = await InvokeEndpoint(command, _mediatorMock);
            // Assert
            Assert.True(result is ValidationProblem || result is ProblemHttpResult);


        }
    }
}
