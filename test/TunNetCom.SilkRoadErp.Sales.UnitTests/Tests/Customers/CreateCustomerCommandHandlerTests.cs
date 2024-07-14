using Moq;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using System.Linq.Expressions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<CreateCustomerCommandHandler>> _mockLogger;
        private readonly CreateCustomerCommandHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "SalesContext")
                .Options;

            _context = new SalesContext(options);
            _mockLogger = new Mock<ILogger<CreateCustomerCommandHandler>>();
            _handler = new CreateCustomerCommandHandler(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_CustomerNameExists_ReturnsFailResult()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                Nom: "Existing Customer",
                Tel: "123456",
                Adresse: "Address",
                Matricule: "Matricule",
                Code: "Code",
                CodeCat: "CodeCat",
                EtbSec: "EtbSec",
                Mail: "email@example.com");

            var clientDuplicated = Client.CreateClient(
                nom: "Existing Customer",
                tel: "123456",
                adresse: "Address",
                matricule: "Matricule",
                code: "Code",
                codeCat: "CodeCat",
                etbSec: "EtbSec",
                mail: "email@example.com");

            _context.Client.Add(clientDuplicated);
            await _context.SaveChangesAsync();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("customer_name_exist", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_NewCustomer_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                Nom: "New Customer",
                Tel: "123456",
                Adresse: "Address",
                Matricule: "Matricule",
                Code: "Code",
                CodeCat: "CodeCat",
                EtbSec: "EtbSec",
                Mail: "email@example.com");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_LogsCustomerCreated()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                Nom: "New Customer",
                Tel: "123456",
                Adresse: "Address",
                Matricule: "Matricule",
                Code: "Code",
                CodeCat: "CodeCat",
                EtbSec: "EtbSec",
                Mail: "email@example.com");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogCustomerCreated(command), Times.Once);
        }

        [Fact]
        public async Task Handle_LogsCustomerCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                Nom: "New Customer",
                Tel: "123456",
                Adresse: "Address",
                Matricule: "Matricule",
                Code: "Code",
                CodeCat: "CodeCat",
                EtbSec: "EtbSec",
                Mail: "email@example.com");

            var client = Client.CreateClient(
                nom: command.Nom,
                tel: command.Tel,
                adresse: command.Adresse,
                matricule: command.Matricule,
                code: command.Code,
                codeCat: command.CodeCat,
                etbSec: command.EtbSec,
                mail: command.Mail);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogCustomerCreatedSuccessfully(client.Id), Times.Once);
        }
    }
}
