using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes.CreateReceiptNote;

public class CreateReceiptNoteEndpointTest
{
    private static async Task<Results<Created<CreateReceiptNoteRequest>, ValidationProblem>> InvokeEndpoint(
        IMediator mediator,
        CreateReceiptNoteRequest request,
        CancellationToken cancellationToken)
    {
        var createCommand = new CreateReceiptNoteCommand(
            request.NumBonFournisseur,
            request.DateLivraison,
            request.IdFournisseur,
            request.Date,
            request.NumFactureFournisseur);
        var result = await mediator.Send(createCommand, cancellationToken);
        if (result.IsFailed)
            return result.ToValidationProblem();
        return TypedResults.Created($"/receiptnotes/{result.Value}", request);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreated()
    {
        var mediatorMock = new Mock<IMediator>();
        var request = new CreateReceiptNoteRequest
        {
            NumBonFournisseur = 123456,
            DateLivraison = DateTime.UtcNow,
            IdFournisseur = 5,
            Date = DateTime.UtcNow,
            NumFactureFournisseur = null
        };
        _ = mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(123));

        var result = await InvokeEndpoint(mediatorMock.Object, request, CancellationToken.None);

        _ = result.Result.Should().BeOfType<Created<CreateReceiptNoteRequest>>();
        var createdResult = result.Result as Created<CreateReceiptNoteRequest>;
        _ = createdResult.Should().NotBeNull();
        createdResult!.Location.Should().Be("/receiptnotes/123");
        createdResult.Value.Should().BeEquivalentTo(request);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ReturnsValidationProblem()
    {
        var mediatorMock = new Mock<IMediator>();
        var request = new CreateReceiptNoteRequest
        {
            NumBonFournisseur = 0,
            DateLivraison = default,
            IdFournisseur = -1,
            Date = default,
            NumFactureFournisseur = null
        };
        var failedResult = Result.Fail("not_found");
        _ = mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateReceiptNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResult);

        var result = await InvokeEndpoint(mediatorMock.Object, request, CancellationToken.None);

        _ = result.Result.Should().BeOfType<ValidationProblem>();
    }
}
