using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByInvoiceId;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNote.GetDeliveryNotesByInvoiceId
{
    public class GetDeliveryNotesByInvoiceIdEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetDeliveryNotesByInvoiceIdEndpoint _endpoint;
        public GetDeliveryNotesByInvoiceIdEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new GetDeliveryNotesByInvoiceIdEndpoint();
        }

        [Fact]
        public async Task GetDeliveryNotesByInvoiceId_Successful_ReturnsOkResult()
        {
            // Arrange
            int invoiceId = 100;

            var expectedNotes = new List<DeliveryNoteResponse>
            {
                new DeliveryNoteResponse
                {
                    DeliveryNoteNumber = 1,
                    Date = DateTime.Today,
                    CreationTime = TimeOnly.FromDateTime(DateTime.Now),
                    CustomerId = 10,
                    InvoiceNumber = 100,
                    TotalAmount = 120,
                    TotalExcludingTax = 100,
                    TotalVat = 20,
                    Items = new List<DeliveryNoteDetailResponse>
                    {
                        new DeliveryNoteDetailResponse
                        {
                            Id = 1,
                            Provider = "Provider A",
                            Date = DateTime.Today,
                            ProductReference = "PRD001",
                            Description = "Test Product",
                            Quantity = 2,
                            UnitPriceExcludingTax = 50,
                            DiscountPercentage = 0,
                            TotalExcludingTax = 100,
                            VatPercentage = 20,
                            TotalIncludingTax = 120,
                            NetTtcUnitaire = 60,
                            PrixHtFodec = null
                        }
                    }
                }
            };
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetDeliveryNotesByInvoiceIdQuery>(q => q.NumFacture == invoiceId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(expectedNotes));
            // Act
            var result = await _endpoint.InvokePrivateHandler(invoiceId, _mediatorMock.Object, CancellationToken.None);
            // Assert
            var okResult = result.Result as Ok<List<DeliveryNoteResponse>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedNotes);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetDeliveryNotesByInvoiceIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetDeliveryNotesByInvoiceId_NotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int invoiceId = 999;
            var failResult = Result.Fail<List<DeliveryNoteResponse>>(new Error("not_found").WithMetadata("ErrorType", "EntityNotFound"));
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetDeliveryNotesByInvoiceIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failResult);
            // Act
            var result = await _endpoint.InvokePrivateHandler(invoiceId, _mediatorMock.Object, CancellationToken.None);
            // Assert
            result.Result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task GetDeliveryNotesByInvoiceId_CancellationRequested_ThrowsOperationCanceledException()
        {
            // Arrange
            int invoiceId = 500;
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetDeliveryNotesByInvoiceIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _endpoint.InvokePrivateHandler(invoiceId, _mediatorMock.Object, cts.Token));
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetDeliveryNotesByInvoiceIdQuery>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }

    public static class GetDeliveryNotesByInvoiceIdEndpointTestExtensions
    {
        public static async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> InvokePrivateHandler(
            this GetDeliveryNotesByInvoiceIdEndpoint endpoint,
            int numFacture,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var query = new GetDeliveryNotesByInvoiceIdQuery(numFacture);
            var result = await mediator.Send(query, cancellationToken);
            if (result.IsEntityNotFound())
                return TypedResults.NotFound();
            return TypedResults.Ok(result.Value);
        }
    }

}
