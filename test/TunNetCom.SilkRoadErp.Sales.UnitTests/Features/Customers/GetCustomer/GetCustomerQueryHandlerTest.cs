namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class GetCustomerQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<GetCustomerQueryHandler>> _loggerMock;
        private readonly GetCustomerQueryHandler _handler;
        public GetCustomerQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: $"SalesTestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<GetCustomerQueryHandler>>();
            _handler = new GetCustomerQueryHandler(_context, _loggerMock.Object);
            SeedDatabase();
        }
        private void SeedDatabase()
        {
            var clients = new List<Client>
            {
                Client.CreateClient("Alice", "1111", "Rue A", "M1", "C1", "Cat1", "E1", "alice@email.com"),
                Client.CreateClient("Bob", "2222", "Rue B", "M2", "C2", "Cat2", "E2", "bob@email.com"),
                Client.CreateClient("Charlie", "3333", "Rue C", "M3", "C3", "Cat3", "E3", "charlie@email.com")
            };
            clients[0].SetId(1);
            clients[1].SetId(2);
            clients[2].SetId(3);
            _context.Client.AddRange(clients);
            _ = _context.SaveChanges();
        }

        [Fact]
        public async Task Handle_ShouldReturnAllCustomers_WhenNoSearchKeyword()
        {
            // Arrange
            var query = new GetCustomerQuery(1, 10, null);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Should().NotBeNull();
            _ = result.Items.Should().HaveCount(3);

            _ = result.PageSize.Should().Be(10);
        }
        [Fact]
        public async Task Handle_ShouldReturnFilteredCustomer_WhenSearchByName()
        {
            // Arrange
            var query = new GetCustomerQuery(1, 10, "Bob");
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Items.Should().ContainSingle(c => c.Name == "Bob");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoMatchFound()
        {
            // Arrange
            var query = new GetCustomerQuery(1, 10, "DoesNotExist");
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldRespectPagination()
        {
            // Arrange: request page size = 2, should return only 2 items
            var query = new GetCustomerQuery(1, 2, null);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.Items.Should().HaveCount(2);
            _ = result.PageSize.Should().Be(2);
        }
    }
}
