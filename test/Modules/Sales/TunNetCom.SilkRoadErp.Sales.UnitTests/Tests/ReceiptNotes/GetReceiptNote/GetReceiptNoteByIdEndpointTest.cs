using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class GetReceiptNoteByIdEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock = new();
        [Fact]
        public async Task Endpoint_ShouldReturnOkWithReceiptNote_WhenFound()
        {
            // Arrange
            const int receiptNoteNum = 123;
            var expectedResponse = new ReceiptNoteResponse
            {
                Num = receiptNoteNum,
                NumBonFournisseur = 456,
                DateLivraison = DateTime.Now,
                IdFournisseur = 789,
                Date = DateTime.Now.AddDays(-1),
                NumFactureFournisseur = 101112
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.Is<GetReceiptNoteByIdQuery>(q => q.Num == receiptNoteNum), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(expectedResponse));
            // Act
            var result = await InvokeEndpoint(receiptNoteNum);
            // Assert
            var okResult = result.Result as Ok<ReceiptNoteResponse>;
            _ = okResult.Should().NotBeNull();
            _ = okResult!.Value.Should().BeEquivalentTo(expectedResponse);
            _mediatorMock.VerifyAll();
        }

        [Fact]
        public async Task Endpoint_ShouldReturnNotFound_WhenReceiptNoteDoesNotExist()
        {
            // Arrange
            const int receiptNoteNum = 999;
            _ = _mediatorMock
                .Setup(m => m.Send(It.Is<GetReceiptNoteByIdQuery>(q => q.Num == receiptNoteNum), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<ReceiptNoteResponse>(EntityNotFound.Error("not_found")));
            // Act
            var result = await InvokeEndpoint(receiptNoteNum);
            // Assert
            var notFoundResult = result.Result as NotFound;
            _ = notFoundResult.Should().NotBeNull();
            _mediatorMock.VerifyAll();
        }
        private async Task<Results<Ok<ReceiptNoteResponse>, NotFound>> InvokeEndpoint(int num)
        {
            var handler = new Func<IMediator, int, CancellationToken, Task<Results<Ok<ReceiptNoteResponse>, NotFound>>>(
                async (mediator, id, token) =>
                {
                    var query = new GetReceiptNoteByIdQuery(id);
                    var result = await mediator.Send(query, token);

                    if (result.IsFailed && result.HasError<EntityNotFound>())
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.Ok(result.Value);
                });

            return await handler(_mediatorMock.Object, num, CancellationToken.None);
        }
    }
}

