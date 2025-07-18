using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
public class GetAppParametersEndpointTest
{
    [Fact]
    public async Task Handle_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var expected = new GetAppParametersResponse
        {
            NomSociete = "TestSociete",
            Timbre = 0.5m,
            Adresse = "AdresseTest",
            Tel = "12345678",
            CodeTva = "TVA20",
            PourcentageFodec = 1.5m,
            PourcentageRetenu = 5,
            DiscountPercentage = 10,
            VatAmount = 19
        };
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAppParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expected));
        var endpoint = new GetAppParametersEndpoint();
        // Act
        var result = await endpoint.Handle(mediatorMock.Object, CancellationToken.None);
        // Assert
        result.Should().BeOfType<Ok<GetAppParametersResponse>>();
        var okResult = (Ok<GetAppParametersResponse>)result;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenMediatorReturnsFailure()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAppParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new List<IError> { new Error("Some error") }));
        var endpoint = new GetAppParametersEndpoint();
        // Act
        var result = await endpoint.Handle(mediatorMock.Object, CancellationToken.None);
        // Assert
        result.Should().BeOfType<BadRequest<List<IReason>>>();
    }
}
