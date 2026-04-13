using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNote;
public class GetDeliveryNotesByClientIdEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Func<IMediator, int, CancellationToken, Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>>> _handler;
    public GetDeliveryNotesByClientIdEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = async (mediator, clientId, ct) =>
        {
            var query = new GetDeliveryNoteByClientIdQuery(clientId);
            var result = await mediator.Send(query, ct);
            if (result.IsFailed || result.Value == null || result.Value.Count == 0)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(result.Value);
        };
    }

    [Fact]
    public async Task Endpoint_ReturnsOkWithDeliveryNotes_WhenClientHasNotes()
    {
        // Arrange
        const int clientId = 1;
        var expectedNotes = new List<DeliveryNoteResponse>
        {
            new()
            {
                DeliveryNoteNumber = 1,
                CustomerId = clientId,
                Date = DateTime.Today,
                Items = new List<DeliveryNoteDetailResponse>
                {
                    new() { ProductReference = "REF123", Quantity = 2 }
                }
            }
        };
        _ = _mediatorMock
            .Setup(m => m.Send(It.Is<GetDeliveryNoteByClientIdQuery>(q => q.ClientId == clientId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedNotes));
        // Act
        var response = await _handler(_mediatorMock.Object, clientId, CancellationToken.None);
        // Assert
        var okResult = Assert.IsType<Ok<List<DeliveryNoteResponse>>>(response.Result);
        Assert.Equal(expectedNotes, okResult.Value);
    }

    [Fact]
    public async Task Endpoint_ReturnsNotFound_WhenClientHasNoNotes()
    {
        // Arrange
        const int clientId = 2;
        _ = _mediatorMock
            .Setup(m => m.Send(It.Is<GetDeliveryNoteByClientIdQuery>(q => q.ClientId == clientId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<DeliveryNoteResponse>()));
        // Act
        var response = await _handler(_mediatorMock.Object, clientId, CancellationToken.None);
        // Assert
        _ = Assert.IsType<NotFound>(response.Result);
    }

    [Fact]
    public async Task Endpoint_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        const int clientId = 999;
        _ = _mediatorMock
            .Setup(m => m.Send(It.Is<GetDeliveryNoteByClientIdQuery>(q => q.ClientId == clientId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<List<DeliveryNoteResponse>>("Client not found"));
        // Act
        var response = await _handler(_mediatorMock.Object, clientId, CancellationToken.None);
        // Assert
        _ = Assert.IsType<NotFound>(response.Result);
    }

    [Fact]
    public async Task Endpoint_ReturnsCorrectNoteDetails_WhenNotesExist()
    {
        // Arrange
        const int clientId = 3;
        var expectedNote = new DeliveryNoteResponse
        {
            DeliveryNoteNumber = 3,
            CustomerId = clientId,
            TotalAmount = 240,
            Items = new List<DeliveryNoteDetailResponse>
            {
                new()
                {
                    ProductReference = "REF789",
                    Description = "Product Test",
                  
                }
            }
        };
        _ = _mediatorMock
            .Setup(m => m.Send(It.Is<GetDeliveryNoteByClientIdQuery>(q => q.ClientId == clientId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<DeliveryNoteResponse> { expectedNote }));
        // Act
        var response = await _handler(_mediatorMock.Object, clientId, CancellationToken.None);
        // Assert
        var okResult = Assert.IsType<Ok<List<DeliveryNoteResponse>>>(response.Result);
        Assert.Equal("REF789 - Product Test", okResult.Value[0].Items[0].ProductReferenceAndDescription);
    }
}