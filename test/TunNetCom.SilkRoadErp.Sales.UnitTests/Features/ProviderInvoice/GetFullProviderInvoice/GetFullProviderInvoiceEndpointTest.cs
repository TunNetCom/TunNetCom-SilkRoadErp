using TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetFullProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class GetFullProviderInvoiceEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetFullProviderInvoiceEndpoint _endpoint;
    public GetFullProviderInvoiceEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new GetFullProviderInvoiceEndpoint();
    }
    [Fact]
    public async Task ShouldReturnOk_WhenInvoiceExists()
    {
        // Arrange
        int invoiceId = 1;
        var response = new FullProviderInvoiceResponse
        {
            ProviderInvoiceNumber = invoiceId,
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetFullProviderInvoiceQuery>(q => q.Id == invoiceId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(response));
        // Act
        var result = await GetFullProviderInvoiceEndpoint.HandleGetFullProviderInvoiceByIdAsync(
            _mediatorMock.Object, invoiceId, CancellationToken.None);
        // Assert
        var okResult = Assert.IsType<Ok<FullProviderInvoiceResponse>>(result.Result);
        Assert.Equal(invoiceId, okResult.Value.ProviderInvoiceNumber);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenInvoiceDoesNotExist()
    {
        // Arrange
        int invoiceId = 404;
        var failResult = Result.Fail<FullProviderInvoiceResponse>(EntityNotFound.Error());
        _mediatorMock.Setup(m => m.Send(It.Is<GetFullProviderInvoiceQuery>(q => q.Id == invoiceId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failResult);
        // Act
        var result = await GetFullProviderInvoiceEndpoint.HandleGetFullProviderInvoiceByIdAsync(
            _mediatorMock.Object, invoiceId, CancellationToken.None);
        // Assert
        Assert.IsType<NotFound>(result.Result);
    }
}
