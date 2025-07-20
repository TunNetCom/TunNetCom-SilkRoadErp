using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNote;
public class GetDeliveryNoteByNumEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Func<IMediator, int, CancellationToken, Task<Results<Ok<DeliveryNoteResponse>, NotFound>>> _handler;
    public GetDeliveryNoteByNumEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = async (mediator, num, ct) =>
        {
            var query = new GetDeliveryNoteByNumQuery(num);
            var result = await mediator.Send(query, ct);
            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(result.Value);
        };
    }

    [Fact]
    public async Task Endpoint_ReturnsOkWithDeliveryNote_WhenNoteExists()
    {
        // Arrange
        const int noteNum = 123;
        var expectedResponse = new DeliveryNoteResponse
        {
            DeliveryNoteNumber = noteNum,
            Date = DateTime.Today,
            CreationTime = TimeOnly.FromDateTime(DateTime.Now),
            TotalAmount = 1000,
            Items = new List<DeliveryNoteDetailResponse>
            {
                new()
                {
                    ProductReference = "REF123",
                    Description = "Test Product",
                    Quantity = 2,
                    UnitPriceExcludingTax = 500
                }
            }
        };
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetDeliveryNoteByNumQuery>(q => q.Num == noteNum), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedResponse));
        // Act
        var response = await _handler(_mediatorMock.Object, noteNum, CancellationToken.None);
        // Assert
        var okResult = Assert.IsType<Ok<DeliveryNoteResponse>>(response.Result);
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Endpoint_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        const int noteNum = 999;
        var failedResult = Result.Fail<DeliveryNoteResponse>("not_found");
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetDeliveryNoteByNumQuery>(q => q.Num == noteNum), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResult);
        // Act
        var response = await _handler(_mediatorMock.Object, noteNum, CancellationToken.None);
        // Assert
        var notFoundResult = Assert.IsType<NotFound>(response.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}
public static class ResultExtensions
{
    public static bool IsEntityNotFound<T>(this Result<T> result)
    {
        return result.IsFailed && result.Errors.Exists(e => e.Message == "not_found");
    }
}
