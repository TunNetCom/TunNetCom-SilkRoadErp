using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class DeleteReceiptNoteEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DeleteReceiptNoteEndpoint _endpoint;
        public DeleteReceiptNoteEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new DeleteReceiptNoteEndpoint();
        }
        private async Task<object> InvokeEndpoint(int num, Mock<IMediator> mediatorMock)
        {
            var deleteCommand = new DeleteReceiptNoteCommand(num);
            var result = await mediatorMock.Object.Send(deleteCommand, CancellationToken.None);
            if (result.IsEntityNotFound())
                return TypedResults.NotFound();
            return TypedResults.NoContent();
        }

        [Fact]
        public async Task Handle_DeleteSuccess_ReturnsNoContent()
        {
            // Arrange
            int numToDelete = 1;
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var result = await InvokeEndpoint(numToDelete, _mediatorMock);
            // Assert
            _ = Assert.IsType<NoContent>(result);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ReturnsNotFound()
        {
            // Arrange
            int numToDelete = 999;
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(EntityNotFound.Error()));
            // Act
            var result = await InvokeEndpoint(numToDelete, _mediatorMock);
            // Assert
            _ = Assert.IsType<NotFound>(result);
        }
    }
}

