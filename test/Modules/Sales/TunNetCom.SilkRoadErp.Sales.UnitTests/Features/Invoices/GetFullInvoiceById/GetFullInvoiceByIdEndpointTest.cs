using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class GetFullInvoiceByIdEndpointTest
{
    [Fact]
    public async Task HandleGetFullInvoiceByIdAsync_ShouldReturnOk_WhenInvoiceExists()
    {
        // Arrange
        var invoiceId = 1;
        var mockMediator = new Mock<IMediator>();
        var deliveryNote = new FullInvoiceCustomerResponseDeliveryNoteResponse
        {
            Num = 101,
            Date = new DateTime(2025, 7, 20),
            TotHTva = 500,
            TotTva = 100,
            NetPayer = 600,
            TempBl = new TimeOnly(10, 30),
            ClientId = 10,
            Lines = new List<DeliveryNoteLineResponse>
            {
                new() {
                    IdLi = 1,
                    RefProduit = "REF001",
                    DesignationLi = "Produit 1",
                    QteLi = 2,
                    PrixHt = 250,
                    Remise = 0,
                    TotHt = 500,
                    Tva = 20,
                    TotTtc = 600
                }
            }
        };
        var expectedResponse = new FullInvoiceResponse
        {
            Num = invoiceId,
            IdClient = 10,
         
            DeliveryNotes = new List<FullInvoiceCustomerResponseDeliveryNoteResponse> { deliveryNote }
        };
        _ = mockMediator
            .Setup(m => m.Send(It.Is<GetFullInvoiceByIdQuery>(q => q.Id == invoiceId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedResponse));
        // Act
        var result = await GetFullInvoiceByIdEndpoint.HandleGetFullInvoiceByIdAsync(mockMediator.Object, invoiceId, CancellationToken.None);
        // Assert
        var okResult = result.Result as Ok<FullInvoiceResponse>;
        _ = okResult.Should().NotBeNull();
        _ = okResult!.Value.Num.Should().Be(invoiceId);
        _ = okResult.Value.DeliveryNotes.Should().HaveCount(1);
    }

    [Fact]
    public async Task HandleGetFullInvoiceByIdAsync_ShouldReturnNotFound_WhenInvoiceNotFound()
    {
        // Arrange
        var invoiceId = 999;
        var mockMediator = new Mock<IMediator>();
        _ = mockMediator
            .Setup(m => m.Send(It.Is<GetFullInvoiceByIdQuery>(q => q.Id == invoiceId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<FullInvoiceResponse>("EntityNotFound"));
        // Act
        var result = await GetFullInvoiceByIdEndpoint.HandleGetFullInvoiceByIdAsync(mockMediator.Object, invoiceId, CancellationToken.None);
        // Assert
        _ = result.Result.Should().BeOfType<NotFound>();
    }
}
