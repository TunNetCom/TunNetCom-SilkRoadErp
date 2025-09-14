using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class UpdateCustomerEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Func<IMediator, int, UpdateCustomerRequest, CancellationToken, Task<Results<NoContent, NotFound, ValidationProblem>>> _handler;
        public UpdateCustomerEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = async (mediator, id, request, ct) =>
            {
                var command = new UpdateCustomerCommand(
                    Id: id,
                    Nom: request.Nom,
                    Tel: request.Tel,
                    Adresse: request.Adresse,
                    Matricule: request.Matricule,
                    Code: request.Code,
                    CodeCat: request.CodeCat,
                    EtbSec: request.EtbSec,
                    Mail: request.Mail);
                var result = await mediator.Send(command, ct);
                if (result.IsEntityNotFound())
                    return TypedResults.NotFound();
                if (result.IsFailed)
                    return result.ToValidationProblem();
                return TypedResults.NoContent();
            };
        }

        [Fact]
        public async Task Endpoint_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var request = new UpdateCustomerRequest
            {
                Nom = "John Doe",
                Tel = "123456",
                Adresse = "Nowhere",
                Matricule = "X123",
                Code = "C123",
                CodeCat = "CC123",
                EtbSec = "ETB",
                Mail = "john@doe.com"
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(EntityNotFound.Error()));
            // Act
            var response = await _handler(_mediatorMock.Object, 999, request, CancellationToken.None);
            // Assert
            var notFoundResult = Assert.IsType<NotFound>(response.Result);
            _ = notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Endpoint_ReturnsValidationProblem_WhenUpdateFails()
        {
            // Arrange
            var request = new UpdateCustomerRequest
            {
                Nom = "John Doe",
                Tel = null,
                Adresse = null,
                Matricule = null,
                Code = null,
                CodeCat = null,
                EtbSec = null,
                Mail = null
            };
            var errors = new List<IError> { new Error("customer_name_exist") };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(errors));
            // Act
            var response = await _handler(_mediatorMock.Object, 1, request, CancellationToken.None);
            // Assert
            var validationResult = Assert.IsType<ValidationProblem>(response.Result);
            _ = validationResult.Should().NotBeNull();
            _ = validationResult.ProblemDetails.Errors.Should().ContainKey("errors");
            _ = validationResult.ProblemDetails.Errors["errors"].Should().Contain("customer_name_exist");
        }
        [Fact]
        public async Task Endpoint_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            var request = new UpdateCustomerRequest
            {
                Nom = "Updated Client",
                Tel = "987654",
                Adresse = "Updated Street",
                Matricule = "M987",
                Code = "C987",
                CodeCat = "CC987",
                EtbSec = "ETB987",
                Mail = "updated@mail.com"
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var response = await _handler(_mediatorMock.Object, 1, request, CancellationToken.None);
            // Assert
            var noContentResult = Assert.IsType<NoContent>(response.Result);
            _ = noContentResult.Should().NotBeNull();
        }
    }
}
