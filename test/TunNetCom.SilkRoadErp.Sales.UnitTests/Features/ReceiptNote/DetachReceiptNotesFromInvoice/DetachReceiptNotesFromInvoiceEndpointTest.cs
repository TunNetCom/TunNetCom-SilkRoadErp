using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class DetachReceiptNotesFromInvoiceEndpointTest
    {
        private static async Task<IResult> InvokeEndpoint(
            IMediator mediator,
            DetachReceiptNotesFromInvoiceCommand cmd,
            CancellationToken ct)
        {
            var result = await mediator.Send(cmd, ct);
            if (result.HasError<EntityNotFound>())
                return TypedResults.NotFound();
            if (result.IsFailed)
                return result.ToValidationProblem();
            return TypedResults.NoContent();
        }

        [Fact]
        public async Task DetachReceiptNotesFromInvoice_ReturnsNotFound_WhenEntityNotFound()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var command = new DetachReceiptNotesFromInvoiceCommand(123, new List<int> { 1, 2 });
            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(EntityNotFound.Error("not_found")));
            // Act
            var result = await InvokeEndpoint(mediatorMock.Object, command, CancellationToken.None);
            // Assert
            var statusCodeResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task DetachReceiptNotesFromInvoice_ReturnsBadRequest_WhenGenericError()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var command = new DetachReceiptNotesFromInvoiceCommand(123, new List<int> { 1, 2 });
            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail("SomeError"));
            // Act
            var result = await InvokeEndpoint(mediatorMock.Object, command, CancellationToken.None);
            // Assert
            var statusCodeResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task DetachReceiptNotesFromInvoice_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var command = new DetachReceiptNotesFromInvoiceCommand(123, new List<int> { 1, 2 });
            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var result = await InvokeEndpoint(mediatorMock.Object, command, CancellationToken.None);
           // Assert
            var statusCodeResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, statusCodeResult.StatusCode);
        }
    }
}
