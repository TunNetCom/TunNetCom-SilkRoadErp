using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.CreateCustomer
{
    public class CreateCustomerEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateCustomerEndpoint _endpoint;
        public CreateCustomerEndpointTest()
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
                Nom = "Test Customer",
                Tel = "123456789",
                Adresse = "Address",
                Matricule = "Mat123",
                Code = "Code123",
                CodeCat = "Cat123",
                EtbSec = "Sec123",
                Mail = "test@mail.com"
            };
            _mediatorMock
                .Setup(m => m.Send(It.Is<CreateCustomerCommand>(cmd =>
                        cmd.Nom == request.Nom &&
                        cmd.Tel == request.Tel &&
                        cmd.Adresse == request.Adresse),
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
        public async Task HandleCreateCustomerAsync_FailedCreation_ReturnsValidationProblem()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Nom = "Duplicate Customer"
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<int>("customer_name_exist"));
            // Act
            var result = await _endpoint.HandleCreateCustomerAsync(
                _mediatorMock.Object,
                request,
                CancellationToken.None);
            // Assert
            var validationProblem = result.Result as ValidationProblem;
            validationProblem.Should().NotBeNull();
            validationProblem!.ProblemDetails.Errors.Values.Should().Contain(errors => errors.Contains("customer_name_exist"));

            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
