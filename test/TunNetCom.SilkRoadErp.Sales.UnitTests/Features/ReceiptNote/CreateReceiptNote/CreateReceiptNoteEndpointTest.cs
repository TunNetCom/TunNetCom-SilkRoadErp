using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes;
public class CreateReceiptNoteEndpointTest
{
    private readonly Mock<IMediator> _mediatorMock;

    public CreateReceiptNoteEndpointTest()
    {
        _mediatorMock = new Mock<IMediator>();
    }
    private static async Task<object> InvokeEndpoint(
        CreateReceiptNoteRequest request,
        Mock<IMediator> mediatorMock)
    {
        var createCommand = new CreateReceiptNoteCommand(
            request.Num,
            request.NumBonFournisseur,
            request.DateLivraison,
            request.IdFournisseur,
            request.Date,
            request.NumFactureFournisseur);
        var result = await mediatorMock.Object.Send(createCommand, CancellationToken.None);
        return result.IsFailed
            ? result.ToValidationProblem()
            : TypedResults.Created($"/receiptnotes/{result.Value}", request);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreated()
    {
        // Arrange
        var request = new CreateReceiptNoteRequest
        {
            Num = 1,
            NumBonFournisseur = 123456,
            DateLivraison = DateTime.UtcNow,
            IdFournisseur = 5,
            Date = DateTime.UtcNow,
            NumFactureFournisseur = null
        };
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(1)); // simulate success
        // Act
        var result = await InvokeEndpoint(request, _mediatorMock);
        // Assert
        var createdResult = Assert.IsType<Created<CreateReceiptNoteRequest>>(result);
        Assert.Equal("/receiptnotes/1", createdResult.Location);
        Assert.Equal(request, createdResult.Value);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ReturnsValidationProblem()
    {
        // Arrange
        var request = new CreateReceiptNoteRequest
        {
            Num = 0,
            NumBonFournisseur = 0,
            DateLivraison = default,
            IdFournisseur = -1,
            Date = default,
            NumFactureFournisseur = null
        };

        var failedResult = Result.Fail("receiptnote_number_exists");
        _ = _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResult);
        // Act
        var result = await InvokeEndpoint(request, _mediatorMock);
        // Assert
        _ = Assert.IsType<ValidationProblem>(result);
    }
}
