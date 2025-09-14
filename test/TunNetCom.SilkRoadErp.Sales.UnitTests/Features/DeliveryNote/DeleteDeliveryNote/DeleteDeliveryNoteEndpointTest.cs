using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
public class DeleteDeliveryNoteEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    public DeleteDeliveryNoteEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
    }
    [Fact]
    public async Task HandleDeleteDeliveryNoteAsync_ShouldReturn_NotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        var deliveryNoteNum = 9999;
        _ = _mediatorMock.Setup(x => x.Send(
                It.IsAny<DeleteDeliveryNoteCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(EntityNotFound.Error()));
        // Act
        var result = await DeleteDeliveryNoteEndpoint.HandleDeleteDeliveryNoteAsync(
            _mediatorMock.Object, deliveryNoteNum, CancellationToken.None);
        // Assert
        _ = result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task HandleDeleteDeliveryNoteAsync_ShouldReturn_NoContent_WhenDeleteSucceeds()
    {
        // Arrange
        var deliveryNoteNum = 123;
        _ = _mediatorMock.Setup(x => x.Send(
                It.IsAny<DeleteDeliveryNoteCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        // Act
        var result = await DeleteDeliveryNoteEndpoint.HandleDeleteDeliveryNoteAsync(
            _mediatorMock.Object, deliveryNoteNum, CancellationToken.None);
        // Assert
        _ = result.Result.Should().BeOfType<NoContent>();
    }
}
