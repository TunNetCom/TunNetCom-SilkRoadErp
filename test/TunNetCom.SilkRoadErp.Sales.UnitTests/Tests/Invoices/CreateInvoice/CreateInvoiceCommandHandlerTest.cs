using System.Reflection.Metadata;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;
using Xunit;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.CreateInvoice
{
    
    public class CreateInvoiceCommandHandlerTest
    {
        private readonly SalesContext _context;
        private readonly TestLogger<CreateInvoiceCommandHandler> _testLogger;
        private readonly CreateInvoiceCommandHandler _createInvoiceCommandhandler;
        public CreateInvoiceCommandHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
           .UseInMemoryDatabase(databaseName: "SalesContext")
       .Options;

            _context = new SalesContext(options);
            _testLogger = new TestLogger<CreateInvoiceCommandHandler>();
            _createInvoiceCommandhandler = new CreateInvoiceCommandHandler(_context, _testLogger);
        }
        [Fact]
        public async Task Handle_ClientDoesNotExist_ReturnsFailResult()
        {
            // Arrange
            var command = new CreateInvoiceCommand(DateTime.Now, ClientId: 999); // client inexistant

            // Act
            var result = await _createInvoiceCommandhandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("not_found", result.Errors.First().Message);
        }
        [Fact]
        public async Task Handle_ValidClient_CreatesInvoiceSuccessfully()
        {
            // Arrange
            var client = Client.CreateClient(
                nom: "Test Client",
                tel: "123456789",
                adresse: "Tunis",
                matricule: "123",
                code: "A1",
                codeCat: "C1",
                etbSec: "001",
                mail: "test@example.com");

            _context.Client.Add(client);
            await _context.SaveChangesAsync();

            var command = new CreateInvoiceCommand(DateTime.Now, client.Id);

            // Act
            var result = await _createInvoiceCommandhandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value > 0); 
        }
    }                                   
}