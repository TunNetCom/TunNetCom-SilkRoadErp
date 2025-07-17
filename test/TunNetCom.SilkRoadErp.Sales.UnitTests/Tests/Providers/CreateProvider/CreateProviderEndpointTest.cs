using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
{
    public class CreateProviderEndPointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        public CreateProviderEndPointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }
        [Fact]
        public async Task CreateProvider_Should_Return_Created_When_Successful()
        {
            // Arrange
            var request = new CreateProviderRequest
            {
                Nom = "Test Provider",
                Tel = "123456789",
                Fax = "12345",
                Matricule = "MAT001",
                Code = "CODE123",
                CodeCat = "CAT001",
                EtbSec = "Sec",
                Mail = "provider@test.com",
                MailDeux = "provider2@test.com",
                Constructeur = true,
                Adresse = "Some Address"
            };
            var command = new CreateProviderCommand(
                Nom: request.Nom,
                Tel: request.Tel,
                Fax: request.Fax,
                Matricule: request.Matricule,
                Code: request.Code,
                CodeCat: request.CodeCat,
                EtbSec: request.EtbSec,
                Mail: request.Mail,
                MailDeux: request.MailDeux,
                Constructeur: request.Constructeur,
                Adresse: request.Adresse);
            _mediatorMock
                .Setup(m => m.Send(It.Is<CreateProviderCommand>(c =>
                    c.Nom == request.Nom && c.Tel == request.Tel), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(1)); // Simulate created with ID = 1
            // Act
            var endpoint = new CreateProviderEndPoint();
            var result = await new CreateProviderEndPoint()
                .HandleRequest(_mediatorMock.Object, request);
            // Assert
            var createdResult = result.Result as Created<CreateProviderRequest>;
            createdResult.Should().NotBeNull();
            createdResult!.Location.Should().Be("/providers/1");
            createdResult.Value.Should().BeEquivalentTo(request);
        }

        [Fact]
        public async Task CreateProvider_Should_Return_BadRequest_When_Failed()
        {
            // Arrange
            var request = new CreateProviderRequest
            {
                Nom = "Test Provider",
                Tel = "123456789",
                Constructeur = true
            };
            var errors = new List<IError> { new Error("nom_invalid") };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateProviderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<int>(errors));
            // Act
            var result = await new CreateProviderEndPoint()
                .HandleRequest(_mediatorMock.Object, request);
            // Assert
            var badRequest = result.Result as BadRequest<List<IError>>;
            badRequest.Should().NotBeNull();
            badRequest!.Value.Should().ContainSingle(e => e.Message == "nom_invalid");
        }
    }
    public static class CreateProviderEndPointExtensions
    {
        public static async Task<Results<Created<CreateProviderRequest>, BadRequest<List<IError>>>> HandleRequest(
            this CreateProviderEndPoint endpoint,
            IMediator mediator,
            CreateProviderRequest request)
        {
            var command = new CreateProviderCommand(
                Nom: request.Nom,
                Tel: request.Tel,
                Fax: request.Fax,
                Matricule: request.Matricule,
                Code: request.Code,
                CodeCat: request.CodeCat,
                EtbSec: request.EtbSec,
                Mail: request.Mail,
                MailDeux: request.MailDeux,
                Constructeur: request.Constructeur,
                Adresse: request.Adresse);
            var result = await mediator.Send(command);
            if (result.IsFailed)
            {
                return TypedResults.BadRequest(result.Errors);
            }
            return TypedResults.Created($"/providers/{result.Value}", request);
        }
    }
}