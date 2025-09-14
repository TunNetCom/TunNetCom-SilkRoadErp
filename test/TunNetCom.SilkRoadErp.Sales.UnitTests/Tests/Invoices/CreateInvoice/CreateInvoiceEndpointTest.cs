using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using FluentResults;
using Moq;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.CreateInvoice
{
    public class CreateInvoiceEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateInvoiceEndpoint _endpoint;

        public CreateInvoiceEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new CreateInvoiceEndpoint();
        }

        [Fact(DisplayName = "HandleCreateInvoiceAsync returns Created result when creation succeeds")]
        public async Task HandleCreateInvoiceAsync_SuccessfulCreation_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateInvoiceRequest
            {
                Date = DateTime.UtcNow,
                ClientId = 1
            };

            _ = _mediatorMock
                .Setup(m => m.Send(It.Is<CreateInvoiceCommand>(cmd =>
                        cmd.Date == request.Date &&
                        cmd.ClientId == request.ClientId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(1));

            // Act
            var result = await _endpoint.HandleCreateInvoiceAsync(
                _mediatorMock.Object,
                request,
                CancellationToken.None);

            // Assert
            _ = result.Should().BeOfType<Results<Created<CreateInvoiceRequest>, ValidationProblem>>();
            var createdResult = result.Result as Created<CreateInvoiceRequest>;
            _ = createdResult.Should().NotBeNull();
            _ = createdResult!.Location.Should().Be("/invoices/1");
            _ = createdResult.Value.Should().BeEquivalentTo(request);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HandleCreateInvoiceAsync returns ValidationProblem when client not found")]
        public async Task HandleCreateInvoiceAsync_InvalidClient_ReturnsValidationProblem()
        {
            // Arrange
            var request = new CreateInvoiceRequest
            {
                Date = DateTime.UtcNow,
                ClientId = 999 // client non existant
            };

            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<int>("not_found"));

            // Act
            var result = await _endpoint.HandleCreateInvoiceAsync(
                _mediatorMock.Object,
                request,
                CancellationToken.None);

            // Assert
            var validationProblem = result.Result as ValidationProblem;
            _ = validationProblem.Should().NotBeNull();

            // Le dictionnaire Errors contient une erreur avec la valeur "not_found"
            _ = validationProblem!.ProblemDetails.Errors.Values
                .SelectMany(errors => errors)
                .Should()
                .Contain(e => e.Contains("not_found"));

            _ = validationProblem.StatusCode.Should().Be(400);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "HandleCreateInvoiceAsync throws OperationCanceledException if cancellation requested")]
        public async Task HandleCreateInvoiceAsync_CancellationRequested_ThrowsOperationCanceledException()
        {
            // Arrange
            var request = new CreateInvoiceRequest
            {
                Date = DateTime.UtcNow,
                ClientId = 1
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            _ = _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            _ = await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _endpoint.HandleCreateInvoiceAsync(_mediatorMock.Object, request, cts.Token));

            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateInvoiceCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
