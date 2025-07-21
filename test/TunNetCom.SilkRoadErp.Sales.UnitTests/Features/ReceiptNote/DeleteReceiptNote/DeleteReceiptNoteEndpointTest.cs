using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class DeleteReceiptNoteEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DeleteReceiptNoteEndpoint _endpoint;
        public DeleteReceiptNoteEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new DeleteReceiptNoteEndpoint();
        }
        private async Task<object> InvokeEndpoint(int num)
        {
            var deleteCommand = new DeleteReceiptNoteCommand(num);
            var result = await _mediatorMock.Object.Send(deleteCommand, CancellationToken.None);
            if (result.IsEntityNotFound())
                return TypedResults.NotFound();
            return TypedResults.NoContent();
        }

        [Fact]
        public async Task Handle_DeleteSuccess_ReturnsNoContent()
        {
            // Arrange
            int numToDelete = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteReceiptNoteCommand>(cmd => cmd.Num == numToDelete), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var result = await InvokeEndpoint(numToDelete);
            // Assert
            Assert.IsType<NoContent>(result);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ReturnsNotFound()
        {
            // Arrange
            int numToDelete = 999;
            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteReceiptNoteCommand>(cmd => cmd.Num == numToDelete), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(EntityNotFound.Error()));
            // Act
            var result = await InvokeEndpoint(numToDelete);
            // Assert
            Assert.IsType<NotFound>(result);
        }
    }
}
