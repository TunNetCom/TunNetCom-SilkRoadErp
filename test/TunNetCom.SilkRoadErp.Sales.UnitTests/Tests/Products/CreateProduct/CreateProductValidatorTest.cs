using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using System.Text.Json;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products
{
    public class CreateProductEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Func<IMediator, CreateProductRequest, CancellationToken, Task<Results<Created<CreateProductRequest>, ValidationProblem>>> _handler;
        public CreateProductEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = async (mediator, request, ct) =>
            {
                var command = new CreateProductCommand(
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
                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }
                return TypedResults.Created($"/products/{result.Value}", request);
            };
        }

        [Fact]
        public async Task Endpoint_ReturnsCreated_WhenMediatorReturnsSuccess()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Refe = "Ref123",
                Nom = "Produit Test",
                Qte = 10,
                QteLimite = 5,
                Remise = 0,
                RemiseAchat = 0,
                Tva = 19,
                Prix = 100,
                PrixAchat = 80,
                Visibilite = true
            };
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok("Ref123"));
            // Act
            var result = await _handler(_mediatorMock.Object, request, CancellationToken.None);
            // Assert
            var created = Assert.IsType<Created<CreateProductRequest>>(result.Result);
            _ = created.Location.Should().Be("/products/Ref123");
            _ = created.Value.Should().BeEquivalentTo(request);
        }

        [Fact]
        public async Task Endpoint_ReturnsValidationProblem_WhenMediatorReturnsFailure()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Refe = "RefExist",
                Nom = "ProduitExist",
                Qte = 15,
                QteLimite = 5,
                Remise = 0,
                RemiseAchat = 0,
                Tva = 19,
                Prix = 100,
                PrixAchat = 80,
                Visibilite = true
            };
            var errorMessage = "product_refe_or_name_exist";
            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<string>(errorMessage));
            // Act
            var result = await _handler(_mediatorMock.Object, request, CancellationToken.None);
            // Assert
            var validation = Assert.IsType<ValidationProblem>(result.Result);
            var json = JsonSerializer.Serialize(validation.ProblemDetails);
            Assert.Contains(errorMessage, json);
        }
    }
}
