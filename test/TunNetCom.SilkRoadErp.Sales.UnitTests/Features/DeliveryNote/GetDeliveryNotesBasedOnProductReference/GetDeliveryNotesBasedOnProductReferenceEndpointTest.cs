//using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;
//using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
//public class GetDeliveryNotesBasedOnProductReferenceEndpointUnitTest
//{
//    private readonly Mock<IMediator> _mediatorMock = new();
//    private GetDeliveryNotesBasedOnProductReferenceEndpoint CreateEndpoint() => new();

//    [Fact]
//    public async Task ShouldReturnBadRequest_WhenProductReferenceIsNullOrWhitespace()
//    {
//        var endpoint = CreateEndpoint();
//        var response1 = await InvokeAsync(endpoint, _mediatorMock.Object, null!, CancellationToken.None);
//        var response2 = await InvokeAsync(endpoint, _mediatorMock.Object, "   ", CancellationToken.None);
//        _ = response1.Should().BeOfType<BadRequest<string>>();
//        _ = response2.Should().BeOfType<BadRequest<string>>();
//    }

//    [Fact]
//    public async Task ShouldReturnOkWithData_WhenProductReferenceIsValid()
//    {
//        // Arrange
//        var productReference = "REF123";
//        var expectedList = new List<DeliveryNoteDetailResponse>
//        {
//            new() { ProductReference = productReference }
//        };
//        _ = _mediatorMock.Setup(m => m.Send(It.IsAny<GetDeliveryNotesBasedOnProductReferenceQuery>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(expectedList);
//        var endpoint = CreateEndpoint();
//        // Act
//        var response = await InvokeAsync(endpoint, _mediatorMock.Object, productReference, CancellationToken.None);
//        // Assert
//        var okResult = response as Ok<List<DeliveryNoteDetailResponse>>;
//        _ = okResult.Should().NotBeNull();
//        _ = okResult!.Value.Should().BeEquivalentTo(expectedList);
//    }

//    private static Task<IResult> InvokeAsync(
//        GetDeliveryNotesBasedOnProductReferenceEndpoint endpoint,
//        IMediator mediator,
//        string productReference,
//        CancellationToken cancellationToken)
//    {     
//        if (string.IsNullOrWhiteSpace(productReference))
//            return Task.FromResult<IResult>(Results.BadRequest("Product reference cannot be null or empty."));
//        var query = new GetDeliveryNotesBasedOnProductReferenceQuery(productReference.Trim());
//        return mediator.Send(query, cancellationToken)
//            .ContinueWith(t => Results.Ok(t.Result), cancellationToken);
//    }
//}
