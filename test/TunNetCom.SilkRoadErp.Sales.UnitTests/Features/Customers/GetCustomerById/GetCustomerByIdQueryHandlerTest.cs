using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class GetCustomerByIdQueryHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldReturnCustomer_WhenClientExists()
        {
            // Arrange
            var client = Client.CreateClient("Ali", "123456", "Tunis", "MAT1", "C1", "Cat1", "E1", "ali@test.com");
            client.SetId(1);
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "GetCustomerById_Exists")
                .Options;
            await using var context = new SalesContext(options);
            _ = context.Client.Add(client);
            _ = await context.SaveChangesAsync();
            var loggerMock = new Mock<ILogger<GetCustomerByIdQueryHandler>>();
            var handler = new GetCustomerByIdQueryHandler(context, loggerMock.Object);
            var query = new GetCustomerByIdQuery(Id: 1);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Id.Should().Be(1);
            _ = result.Value.Name.Should().Be("Ali");
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenClientNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "GetCustomerById_NotFound")
                .Options;
            await using var context = new SalesContext(options);
            var loggerMock = new Mock<ILogger<GetCustomerByIdQueryHandler>>();
            var handler = new GetCustomerByIdQueryHandler(context, loggerMock.Object);
            var query = new GetCustomerByIdQuery(Id: 999); // non-existent ID
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            _ = result.IsFailed.Should().BeTrue();
            _ = result.Errors.Should().Contain(e => e.Message == "not_found");
        }
    }
}
