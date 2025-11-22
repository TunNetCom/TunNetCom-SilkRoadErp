using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class DetachReceiptNotesFromInvoiceEndpointTests
    {
        [Fact]
        public async Task DetachReceiptNotesFromInvoice_ReturnsNotFound_WhenEntityNotFound()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            var command = new DetachReceiptNotesFromInvoiceCommand(
                InvoiceId: 123,
                ReceiptNoteIds: new List<int> { 1, 2 }
            );
            _ = mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(EntityNotFound.Error("not_found")));
            async Task<IResult> EndpointHandler(IMediator mediator, DetachReceiptNotesFromInvoiceCommand cmd, CancellationToken ct)
            {
                var result = await mediator.Send(cmd, ct);
                if (result.HasError<EntityNotFound>())
                    return TypedResults.NotFound();
                if (result.IsFailed)
                    return result.ToValidationProblem();
                return TypedResults.NoContent();
            }
            // Act
            var result = await EndpointHandler(mediatorMock.Object, command, CancellationToken.None);
            // Assert
            var statusCodeResult = result as IStatusCodeHttpResult;
            Assert.NotNull(statusCodeResult);
            Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task DetachReceiptNotesFromInvoice_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var command = new DetachReceiptNotesFromInvoiceCommand(
                InvoiceId: 123,
                ReceiptNoteIds: new List<int> { 1, 2 }
            );
            _ = mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            async Task<IResult> EndpointHandler(IMediator mediator, DetachReceiptNotesFromInvoiceCommand cmd, CancellationToken ct)
            {
                var result = await mediator.Send(cmd, ct);
                if (result.HasError<EntityNotFound>())
                    return TypedResults.NotFound();
                if (result.IsFailed)
                    return result.ToValidationProblem();
                return TypedResults.NoContent();
            }
            // Act
            var result = await EndpointHandler(mediatorMock.Object, command, CancellationToken.None);
            // Assert
            var statusCodeResult = result as IStatusCodeHttpResult;
            Assert.NotNull(statusCodeResult);
            Assert.Equal(StatusCodes.Status204NoContent, statusCodeResult.StatusCode);
        }
    }
}

