using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNotes;

public class AttachToInvoiceEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AttachToInvoiceEndpoint _endpoint;
    public AttachToInvoiceEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new AttachToInvoiceEndpoint();
    }
    [Fact]
    public async Task HandleAttachToInvoiceAsync_Should_Return_NoContent_When_Success()
    {
        // Arrange
        var request = new AttachToInvoiceRequest
        {
            InvoiceId = 1,
            DeliveryNoteIds = new List<int> { 10, 20 }
        };
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<AttachToInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        // Act
        var result = await _endpoint.HandleAttachToInvoiceAsync(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        _ = result.Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task HandleAttachToInvoiceAsync_Should_Return_NotFound_When_EntityNotFound()
    {
        // Arrange
        var request = new AttachToInvoiceRequest
        {
            InvoiceId = 1,
            DeliveryNoteIds = new List<int> { 10, 20 }
        };
        var resultWithError = Result.Fail(EntityNotFound.Error("Invoice not found"));
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<AttachToInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultWithError);
        // Act
        var result = await _endpoint.HandleAttachToInvoiceAsync(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        _ = result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task HandleAttachToInvoiceAsync_Should_Return_ValidationProblem_When_OtherFailure()
    {
        // Arrange
        var request = new AttachToInvoiceRequest
        {
            InvoiceId = 1,
            DeliveryNoteIds = new List<int> { 10, 20 }
        };
        var resultWithError = Result.Fail("invalid_data");
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<AttachToInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultWithError);
        // Act
        var result = await _endpoint.HandleAttachToInvoiceAsync(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        _ = result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>();
    }
}
