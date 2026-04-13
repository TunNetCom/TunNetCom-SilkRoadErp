namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.CreateCustomer
{
    public class CreateCustomerCommandHandlerTest
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<CreateCustomerCommandHandler>> _loggerMock;
        private readonly CreateCustomerCommandHandler _handler;
        public CreateCustomerCommandHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "CreateCustomerTestDb")
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();
            _handler = new CreateCustomerCommandHandler(_context, _loggerMock.Object);
        }
        [Fact]
        public async Task Handle_WhenCustomerNameExists_ReturnsFailure()
        {
            // Arrange
            var existingCustomer = Client.CreateClient(
                "ExistingCustomer",
                "12345678",
                "Address",
                "Matricule",
                "Code",
                "CodeCat",
                "EtbSec",
                "mail@example.com"
            );
            _ = _context.Client.Add(existingCustomer);
            _ = await _context.SaveChangesAsync();
            var command = new CreateCustomerCommand(
                Nom: "ExistingCustomer",
                Tel: "00000000",
                Adresse: "New Address",
                Matricule: "NewMatricule",
                Code: "NewCode",
                CodeCat: "NewCodeCat",
                EtbSec: "NewEtbSec",
                Mail: "newmail@example.com"
           );
           // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            _ = result.IsFailed.Should().BeTrue();
            _ = result.Errors.Should().Contain(e => e.Message == "customer_name_exist");
        }

        [Fact]
        public async Task Handle_WhenNewCustomer_CreatesSuccessfully_ReturnsNewId()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                Nom: "NewCustomer",
                Tel: "123456789",
                Adresse: "Some Address",
                Matricule: "Mat123",
                Code: "Code123",
                CodeCat: "Cat123",
                EtbSec: "Sec123",
                Mail: "test@mail.com"
            );
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().BeGreaterThan(0);
            var createdClient = await _context.Client.FirstOrDefaultAsync(c => c.Id == result.Value);
            _ = createdClient.Should().NotBeNull();
            _ = createdClient!.Nom.Should().Be(command.Nom);
        }
    }
}
