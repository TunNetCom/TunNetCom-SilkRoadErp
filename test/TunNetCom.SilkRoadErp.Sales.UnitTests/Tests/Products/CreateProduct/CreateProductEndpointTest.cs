using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products;
public class CreateProductEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Func<IMediator, CreateProductRequest, CancellationToken, Task<
        Results<Created<CreateProductRequest>, ValidationProblem>>> _handler;
    public CreateProductEndpointTests()
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
                return result.ToValidationProblem(); // Extension method dans ResultExtensions
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
            Refe = "RefTest123",
            Nom = "ProduitTest",
            Qte = 10,
            QteLimite = 5,
            Remise = 0,
            RemiseAchat = 0,
            Tva = 19,
            Prix = 100,
            PrixAchat = 80,
            Visibilite = true
        };
        var expectedRef = "RefTest123";
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedRef));
        // Act
        var response = await _handler(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        var created = Assert.IsType<Created<CreateProductRequest>>(response.Result);
        Assert.Equal($"/products/{expectedRef}", created.Location);
        Assert.Equal(request, created.Value);
    }

    [Fact]
    public async Task Endpoint_ReturnsValidationProblem_WhenProductExists()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Refe = "RefExists",
            Nom = "ProduitExists",
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
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<string>(errorMessage));
        // Act
        var response = await _handler(_mediatorMock.Object, request, CancellationToken.None);
        // Assert
        var validation = Assert.IsType<ValidationProblem>(response.Result);
        // Vérifie que le message d’erreur attendu est bien dans le contenu
        var json = JsonSerializer.Serialize(validation.ProblemDetails);
        Assert.Contains(errorMessage, json);
    }
}
