using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class UpdateReceiptNoteEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        public UpdateReceiptNoteEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }
        [Fact]
        public async Task Endpoint_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var request = new UpdateReceiptNoteRequest
            {
                Num = 1,
                NumBonFournisseur = 100,
                DateLivraison = DateTime.Today,
                IdFournisseur = 200,
                Date = DateTime.Today.AddDays(-1),
                NumFactureFournisseur = 300
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var result = await InvokeEndpoint(request.Num, request);
            // Assert
            _ = result.Result.Should().BeOfType<NoContent>();
        }

        [Fact]
        public async Task Endpoint_ShouldReturnNotFound_WhenReceiptNoteNotFound()
        {
            // Arrange
            var request = new UpdateReceiptNoteRequest
            {
                Num = 2,
                NumBonFournisseur = 101,
                DateLivraison = DateTime.Today,
                IdFournisseur = 201,
                Date = DateTime.Today.AddDays(-2),
                NumFactureFournisseur = 301
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(EntityNotFound.Error()));
            // Act
            var result = await InvokeEndpoint(request.Num, request);
            // Assert
            _ = result.Result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task Endpoint_ShouldReturnValidationProblem_WhenUpdateFailsForOtherReasons()
        {
            // Arrange
            var request = new UpdateReceiptNoteRequest
            {
                Num = 3,
                NumBonFournisseur = 102,
                DateLivraison = DateTime.Today,
                IdFournisseur = 202,
                Date = DateTime.Today.AddDays(-3),
                NumFactureFournisseur = 302
            };
            var error = new Error("validation error").WithMetadata("field", "NumBonFournisseur");
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(error));
            // Act
            var result = await InvokeEndpoint(request.Num, request);
            // Assert
            _ = result.Result.Should().BeOfType<ValidationProblem>();
        }

        private async Task<Results<NoContent, NotFound, ValidationProblem>> InvokeEndpoint(int num, UpdateReceiptNoteRequest request)
        {
            var endpoint = new Func<IMediator, int, UpdateReceiptNoteRequest, CancellationToken, Task<Results<NoContent, NotFound, ValidationProblem>>>(
                async (mediator, id, req, token) =>
                {
                    var command = new UpdateReceiptNoteCommand(
                        Num: req.Num,
                        NumBonFournisseur: req.NumBonFournisseur,
                        DateLivraison: req.DateLivraison,
                        IdFournisseur: req.IdFournisseur,
                        Date: req.Date,
                        NumFactureFournisseur: req.NumFactureFournisseur);
                    var result = await mediator.Send(command, token);
                    if (result.IsEntityNotFound())
                        return TypedResults.NotFound();
                    if (result.IsFailed)
                        return result.ToValidationProblem();
                    return TypedResults.NoContent();
                });
            return await endpoint(_mediatorMock.Object, num, request, CancellationToken.None);
        }
    }
}
