using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.Endpoints;

public class UpdateCustomerEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;

    public UpdateCustomerEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task HandleUpdateCustomerAsync_CustomerNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var request = new UpdateCustomerRequest(
            nom: "Bob",
            tel: "123456",
            adresse: "Bob's House",
            matricule: "BOB001",
            code: "CODE001",
            codeCat: "CAT001",
            etbSec: "SEC001",
            mail: "bob@example.com");

        var result = Result.Fail(EntityNotFound.Error());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await UpdateCustomerEndpoint.HandleUpdateCustomerAsync(
            _mediatorMock.Object,
            id: 1,
            request,
            CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<NotFound>("Bob's gone missing, no update for him!");
    }

    [Fact]
    public async Task HandleUpdateCustomerAsync_DuplicateName_ReturnsValidationProblemResult()
    {
        // Arrange
        var request = new UpdateCustomerRequest(
            nom: "Alice",
            tel: "456789",
            adresse: "Alice's Place",
            matricule: "ALICE001",
            code: "CODE002",
            codeCat: "CAT002",
            etbSec: "SEC002",
            mail: "alice@example.com");

        var result = Result.Fail("customer_name_exist");
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await UpdateCustomerEndpoint.HandleUpdateCustomerAsync(
            _mediatorMock.Object,
            id: 1,
            request,
            CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<ValidationProblem>("Alice can't steal someone else's name!");
    }

    [Fact]
    public async Task HandleUpdateCustomerAsync_SuccessfulUpdate_ReturnsNoContentResult()
    {
        // Arrange
        var request = new UpdateCustomerRequest(
            nom: "Charlie",
            tel: "789101",
            adresse: "Charlie's Cave",
            matricule: "CHARLIE001",
            code: "CODE003",
            codeCat: "CAT003",
            etbSec: "SEC003",
            mail: "charlie@example.com");

        var result = Result.Ok();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await UpdateCustomerEndpoint.HandleUpdateCustomerAsync(
            _mediatorMock.Object,
            id: 1,
            request,
            CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<NoContent>("Charlie's update party was a smashing success!");
    }

    [Fact]
    public async Task HandleUpdateCustomerAsync_MapsRequestToCommand_Correctly()
    {
        // Arrange
        var request = new UpdateCustomerRequest(
            nom: "Dana",
            tel: "101112",
            adresse: "Dana's Den",
            matricule: "DANA001",
            code: "CODE004",
            codeCat: "CAT004",
            etbSec: "SEC004",
            mail: "dana@example.com");

        UpdateCustomerCommand capturedCommand = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result>, CancellationToken>((command, ct) => capturedCommand = (UpdateCustomerCommand)command)
            .ReturnsAsync(Result.Ok());

        // Act
        await UpdateCustomerEndpoint.HandleUpdateCustomerAsync(
            _mediatorMock.Object,
            id: 1,
            request,
            CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull("Dana's command better be there!");
        capturedCommand.Id.Should().Be(1, "Dana's ID got lost in translation!");
        capturedCommand.Nom.Should().Be("Dana", "Dana's name took a vacation?");
        capturedCommand.Tel.Should().Be("101112", "Wrong number for Dana!");
        capturedCommand.Adresse.Should().Be("Dana's Den", "Dana's moving where?");
        capturedCommand.Matricule.Should().Be("DANA001", "Dana's matricule got scrambled!");
        capturedCommand.Code.Should().Be("CODE004", "Dana's code got decoded!");
        capturedCommand.CodeCat.Should().Be("CAT004", "Dana's category got uncategorized!");
        capturedCommand.EtbSec.Should().Be("SEC004", "Dana's sector got misplaced!");
        capturedCommand.Mail.Should().Be("dana@example.com", "Dana's email got spammed!");
    }
}