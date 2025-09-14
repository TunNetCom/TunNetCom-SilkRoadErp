namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class DeleteCustomerHandlerSimpleTest : IDisposable
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<DeleteCustomerCommandHandler>> _loggerMock;
        private readonly DeleteCustomerCommandHandler _handler;
        public DeleteCustomerHandlerSimpleTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<DeleteCustomerCommandHandler>>();
            _handler = new DeleteCustomerCommandHandler(_context, _loggerMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldDeleteCustomer_WhenCustomerExists()
        {
            // Arrange
            var client = Client.CreateClient(
                "Alice",
                "987654321",
                "1 rue test",
                "MAT002",
                "CL001",
                "CAT01",
                "ETB01",
                "alice@test.com");
            _ = _context.Client.Add(client);
            _ = await _context.SaveChangesAsync();
            var command = new DeleteCustomerCommand(client.Id);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            _ = result.IsSuccess.Should().BeTrue();
            var deletedClient = await _context.Client.FindAsync(client.Id);
            _ = deletedClient.Should().BeNull("le client doit être supprimé");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCustomerDoesNotExist()
        {
            // Arrange
            int nonExistentId = 9999;
            var command = new DeleteCustomerCommand(nonExistentId);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            _ = result.IsFailed.Should().BeTrue();
            _ = result.Errors.Should().Contain(e => e.Message == "not_found");
        }

        public void Dispose()
        {
            _ = _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
