using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DetachFromInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class DetachFromInvoiceEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DetachFromInvoiceEndpoint _endpoint;
    public DetachFromInvoiceEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new DetachFromInvoiceEndpoint();
    }
    [Fact]
    public async Task DetachFromInvoice_Success_ReturnsNoContent()
    {
        // Arrange
        var request = new DetachFromInvoiceRequest { InvoiceId = 1, DeliveryNoteIds = new List<int> { 10, 20 } };
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetachFromInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        // Act
        var handler = GetHandler();
        var result = await handler(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        _ = result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task DetachFromInvoice_EntityNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new DetachFromInvoiceRequest { InvoiceId = 999, DeliveryNoteIds = new List<int> { 100 } };
        var error = EntityNotFound.Error();
        var resultWithError = Result.Fail(error);
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetachFromInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultWithError);
        // Act
        var handler = GetHandler();
        var result = await handler(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        _ = result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task DetachFromInvoice_ValidationError_ReturnsValidationProblem()
    {
        // Arrange
        var request = new DetachFromInvoiceRequest { InvoiceId = 1, DeliveryNoteIds = new List<int> { 1 } };
        var result = Result.Fail("Validation failed");
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetachFromInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        // Act
        var handler = GetHandler();
        var resultActual = await handler(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        _ = resultActual.Should().BeOfType<ValidationProblem>();
    }

    private static Func<IMediator, DetachFromInvoiceRequest, CancellationToken, Task<IResult>> GetHandler()
    {
        // Rebuild the inline handler method defined in endpoint registration
        return async (mediator, request, cancellationToken) =>
        {
            var command = new DetachFromInvoiceCommand(request.InvoiceId, request.DeliveryNoteIds);
            var result = await mediator.Send(command, cancellationToken);
            if (result.HasError<EntityNotFound>())
                return TypedResults.NotFound();
            if (result.IsFailed)
                return result.ToValidationProblem();
            return TypedResults.NoContent();
        };
    }
}
