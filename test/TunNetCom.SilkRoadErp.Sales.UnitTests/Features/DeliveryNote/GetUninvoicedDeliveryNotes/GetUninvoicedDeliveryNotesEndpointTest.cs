using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNotes;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Endpoints.DeliveryNote;
public class GetUninvoicedDeliveryNotesEndpointTest
{
    [Fact]
    public async Task Handle_ShouldReturnOk_WhenDeliveryNotesExist()
    {
        // Arrange
        var clientId = 1;
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = new CancellationToken();
        var deliveryNotes = new List<DeliveryNoteResponse>
        {
            new DeliveryNoteResponse { DeliveryNoteNumber = 1,  },
            new DeliveryNoteResponse { DeliveryNoteNumber = 2,  }
        };
        mockMediator
            .Setup(m => m.Send(It.Is<GetUninvoicedDeliveryNotesQuery>(q => q.CustomerId == clientId), cancellationToken))
            .ReturnsAsync(Result.Ok(deliveryNotes));
        var endpoint = new GetUninvoicedDeliveryNotesEndpoint();
        // Act
        var result = await InvokeHandler(endpoint, clientId, mockMediator.Object, cancellationToken);
        // Assert
        result.Result.Should().BeOfType<Ok<List<DeliveryNoteResponse>>>();
        var okResult = result.Result as Ok<List<DeliveryNoteResponse>>;
        okResult!.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenNoDeliveryNotesFound()
    {
        // Arrange
        var clientId = 1;
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = new CancellationToken();
        mockMediator
            .Setup(m => m.Send(It.Is<GetUninvoicedDeliveryNotesQuery>(q => q.CustomerId == clientId), cancellationToken))
            .ReturnsAsync(Result.Ok(new List<DeliveryNoteResponse>()));
        var endpoint = new GetUninvoicedDeliveryNotesEndpoint();
        // Act
        var result = await InvokeHandler(endpoint, clientId, mockMediator.Object, cancellationToken);
        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    private static async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> InvokeHandler(
     GetUninvoicedDeliveryNotesEndpoint endpoint,
     int clientId,
     IMediator mediator,
     CancellationToken cancellationToken)
    {
        var query = new GetUninvoicedDeliveryNotesQuery(clientId);
        var result = await mediator.Send(query, cancellationToken);
        if (result.IsFailed || result.Value == null || result.Value.Count == 0)
           return TypedResults.NotFound();
        return TypedResults.Ok(result.Value);
    }
}
