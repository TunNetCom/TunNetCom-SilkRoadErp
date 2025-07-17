using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products
{
    public class UpdateProductEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Func<IMediator, string, UpdateProductRequest, CancellationToken, Task<Results<NoContent, NotFound, BadRequest<List<IError>>>>> _handler;
        public UpdateProductEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = async (mediator, refe, request, ct) =>
            {
                var command = new UpdateProductCommand(
                    request.Refe,
                    request.Nom,
                    request.Qte,
                    request.QteLimite,
                    request.Remise,
                    request.RemiseAchat,
                    request.Tva,
                    request.Prix,
                    request.PrixAchat,
                    request.Visibilite);
                var result = await mediator.Send(command, ct);
                if (result.IsEntityNotFound())
                    return TypedResults.NotFound();
                if (result.IsFailed)
                    return TypedResults.BadRequest(result.Errors);
                return TypedResults.NoContent();
            };
        }

        [Fact]
        public async Task Endpoint_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var refe = "nonexistent";
            var request = new UpdateProductRequest
            {
                Refe = refe,
                Nom = "Test Product",
                Qte = 10,
                QteLimite = 5,
                Remise = 0,
                RemiseAchat = 0,
                Tva = 19,
                Prix = 100,
                PrixAchat = 80,
                Visibilite = true
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new List<IError> { EntityNotFound.Error() }));
            // Act
            var response = await _handler(_mediatorMock.Object, refe, request, CancellationToken.None);
            // Assert
            var notFoundResult = Assert.IsType<NotFound>(response.Result);
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Endpoint_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var refe = "existingRef";
            var request = new UpdateProductRequest
            {
                Refe = refe,
                Nom = "Test Product",
                Qte = 10,
                QteLimite = 5,
                Remise = 0,
                RemiseAchat = 0,
                Tva = 19,
                Prix = 100,
                PrixAchat = 80,
                Visibilite = true
            };
            var errors = new List<IError> { new FluentResults.Error("invalid_data") };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(errors));
            // Act
            var response = await _handler(_mediatorMock.Object, refe, request, CancellationToken.None);
            // Assert
            var badRequestResult = Assert.IsType<BadRequest<List<IError>>>(response.Result);
            badRequestResult.Should().NotBeNull();
            Assert.Contains(badRequestResult.Value, e => e.Message == "invalid_data");
        }

        [Fact]
        public async Task Endpoint_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            var refe = "existingRef";
            var request = new UpdateProductRequest
            {
                Refe = refe,
                Nom = "Test Product",
                Qte = 10,
                QteLimite = 5,
                Remise = 0,
                RemiseAchat = 0,
                Tva = 19,
                Prix = 100,
                PrixAchat = 80,
                Visibilite = true
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // Act
            var response = await _handler(_mediatorMock.Object, refe, request, CancellationToken.None);
            // Assert
            var noContentResult = Assert.IsType<NoContent>(response.Result);
            noContentResult.Should().NotBeNull();
        }
    }
}