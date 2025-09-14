using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
{
    public class UpdateProviderEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock = new();

        [Fact]
        public async Task UpdateProvider_ReturnsNoContent_WhenSuccessful()
        {
            var request = new UpdateProviderRequest
            {
                Nom = "Test",
                Tel = "123",
                Fax = "456",
                Matricule = "M123",
                Code = "C001",
                CodeCat = "CAT1",
                EtbSec = "ETB",
                Mail = "test@mail.com",
                MailDeux = "test2@mail.com",
                Constructeur = true,
                Adresse = "Adresse"
            };

            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateProviderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());

            var handler = GetHandler();

            var result = await handler(_mediatorMock.Object, 1, request, CancellationToken.None);

            _ = result.Should().BeOfType<Results<NoContent, NotFound, ValidationProblem>>();
            _ = result.Result.Should().BeOfType<NoContent>();
        }

        [Fact]
        public async Task UpdateProvider_ReturnsValidationProblem_WhenValidationFails()
        {
            var request = new UpdateProviderRequest(); // champ vide = échec
            var validationError = Result.Fail("validation_error");

            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateProviderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationError);

            var handler = GetHandler();

            var result = await handler(_mediatorMock.Object, 1, request, CancellationToken.None);

            _ = result.Should().BeOfType<Results<NoContent, NotFound, ValidationProblem>>();
            _ = result.Result.Should().BeOfType<ValidationProblem>();
        }

        [Fact]
        public async Task UpdateProvider_ReturnsNotFound_WhenProviderDoesNotExist()
        {
            var request = new UpdateProviderRequest(); // peut être vide, on teste l’erreur EntityNotFound
            var notFoundResult = Result.Fail(EntityNotFound.Error());

            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateProviderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notFoundResult);

            var handler = GetHandler();

            var result = await handler(_mediatorMock.Object, 1, request, CancellationToken.None);

            _ = result.Should().BeOfType<Results<NoContent, NotFound, ValidationProblem>>();
            _ = result.Result.Should().BeOfType<NotFound>();
        }

        private static Func<IMediator, int, UpdateProviderRequest, CancellationToken, Task<Results<NoContent, NotFound, ValidationProblem>>> GetHandler()
        {
            return async (mediator, id, request, ct) =>
            {
                var command = new UpdateProviderCommand(
                    Id: id,
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

                var result = await mediator.Send(command, ct);

                if (result.HasError<EntityNotFound>())
                    return TypedResults.NotFound();

                if (result.IsFailed)
                    return result.ToValidationProblem();

                return TypedResults.NoContent();
            };
        }
    }
}
