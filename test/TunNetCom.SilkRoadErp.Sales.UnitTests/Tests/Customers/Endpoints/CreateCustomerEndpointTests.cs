using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.Endpoints;

public class CreateCustomerEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CreateCustomerEndpoint _endpoint;

    public CreateCustomerEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new CreateCustomerEndpoint();
    }

    [Fact]
    public async Task HandleCreateCustomerAsync_SuccessfulCreation_ReturnsCreatedResult()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Nom = "John Doe",
            Tel = "123456789",
            Adresse = "123 Main St",
            Matricule = "MAT123",
            Code = "CODE123",
            CodeCat = "CAT1",
            EtbSec = "SEC1",
            Mail = "john@example.com"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateCustomerCommand>(cmd =>
                cmd.Nom == request.Nom &&
                cmd.Tel == request.Tel &&
                cmd.Adresse == request.Adresse &&
                cmd.Matricule == request.Matricule &&
                cmd.Code == request.Code &&
                cmd.CodeCat == request.CodeCat &&
                cmd.EtbSec == request.EtbSec &&
                cmd.Mail == request.Mail),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(1));

        // Act
        var result = await _endpoint.HandleCreateCustomerAsync(
            _mediatorMock.Object,
            request,
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<Results<Created<CreateCustomerRequest>, ValidationProblem>>();
        var createdResult = result.Result as Created<CreateCustomerRequest>;
        createdResult.Should().NotBeNull();
        createdResult!.Location.Should().Be("/customers/1");
        createdResult.Value.Should().BeEquivalentTo(request);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task HandleCreateCustomerAsync_FailedCreationWithSingleError_ReturnsValidationProblem()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Nom = "John Doe",
            Tel = "123456789",
            Adresse = "123 Main St",
            Matricule = "MAT123",
            Code = "CODE123",
            CodeCat = "CAT1",
            EtbSec = "SEC1",
            Mail = "invalid-email"
        };

        var error = new Error("Invalid email format").WithMetadata("errorCode", "Email.Invalid");
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<int>(error));

        // Act
        var result = await _endpoint.HandleCreateCustomerAsync(
            _mediatorMock.Object,
            request,
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<Results<Created<CreateCustomerRequest>, ValidationProblem>>();
        var validationProblem = result.Result as ValidationProblem;
        validationProblem.Should().NotBeNull();
        validationProblem!.StatusCode.Should().Be(400);
        validationProblem.ProblemDetails.Errors.First().Value.Should().Contain("Invalid email format");
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task HandleCreateCustomerAsync_FailedCreationWithMultipleErrors_ReturnsValidationProblem()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Nom = "", // Invalid
            Tel = "invalid-phone",
            Adresse = "123 Main St",
            Matricule = "MAT123",
            Code = "CODE123",
            CodeCat = "CAT1",
            EtbSec = "SEC1",
            Mail = "invalid-email"
        };

        var errors = new List<IError>
        {
            new Error("Name is required").WithMetadata("errorCode", "Nom.Required"),
            new Error("Invalid phone format").WithMetadata("errorCode", "Tel.Invalid"),
            new Error("Invalid email format").WithMetadata("errorCode", "Mail.Invalid")
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<int>(errors));

        // Act
        var result = await _endpoint.HandleCreateCustomerAsync(
            _mediatorMock.Object,
            request,
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<Results<Created<CreateCustomerRequest>, ValidationProblem>>();
        var validationProblem = result.Result as ValidationProblem;
        validationProblem.Should().NotBeNull();
        validationProblem!.StatusCode.Should().Be(400);
        validationProblem.ProblemDetails.Errors.First().Value.Should().HaveCount(3);
        validationProblem.ProblemDetails.Errors.First().Value.Should().Contain("Name is required");
        validationProblem.ProblemDetails.Errors.First().Value.Should().Contain("Invalid phone format");
        validationProblem.ProblemDetails.Errors.First().Value.Should().Contain("Invalid email format");
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task HandleCreateCustomerAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Nom = "John Doe",
            Tel = "123456789",
            Adresse = "123 Main St",
            Matricule = "MAT123",
            Code = "CODE123",
            CodeCat = "CAT1",
            EtbSec = "SEC1",
            Mail = "john@example.com"
        };

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _endpoint.HandleCreateCustomerAsync(
                _mediatorMock.Object,
                request,
                cts.Token));

        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    //[Fact]
    //public async Task HandleCreateCustomerAsync_NullRequest_ThrowsArgumentNullException()
    //{
    //    // Arrange
    //    CreateCustomerRequest? request = null;

    //    // Act & Assert
    //    await Assert.ThrowsAsync<ArgumentNullException>(() =>
    //        _endpoint.HandleCreateCustomerAsync(
    //            _mediatorMock.Object,
    //            request!,
    //            CancellationToken.None));

    //    _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    //}
}