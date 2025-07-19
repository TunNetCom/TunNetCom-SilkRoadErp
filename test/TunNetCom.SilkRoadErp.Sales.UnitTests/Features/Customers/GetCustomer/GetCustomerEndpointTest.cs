namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class GetCustomerQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnPagedCustomers_WhenQueryIsValid()
        {
            // Arrange
            var clients = new List<Client>
            {
                Client.CreateClient("Ali", "123", "Tunis", "MAT1", "C1", "Cat1", "E1", "ali@test.com"),
                Client.CreateClient("Sami", "456", "Sfax", "MAT2", "C2", "Cat2", "E2", "sami@test.com")
            };
            clients[0].SetId(1);
            clients[1].SetId(2);
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "GetCustomersTest")
                .Options;
            await using var context = new SalesContext(options);
            context.Client.AddRange(clients);
            await context.SaveChangesAsync();
            var loggerMock = new Mock<ILogger<GetCustomerQueryHandler>>();
            var handler = new GetCustomerQueryHandler(context, loggerMock.Object);
            var query = new GetCustomerQuery(
                PageNumber: 1,
                PageSize: 2,
                SearchKeyword: null
            );
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.PageSize.Should().Be(2);     
        }
    }
}
