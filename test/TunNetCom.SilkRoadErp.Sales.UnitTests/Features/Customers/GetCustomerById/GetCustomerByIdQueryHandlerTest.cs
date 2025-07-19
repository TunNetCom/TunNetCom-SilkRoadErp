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
            context.Client.Add(client);
            await context.SaveChangesAsync();
            var loggerMock = new Mock<ILogger<GetCustomerByIdQueryHandler>>();
            var handler = new GetCustomerByIdQueryHandler(context, loggerMock.Object);
            var query = new GetCustomerByIdQuery(Id: 1);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.Name.Should().Be("Ali");
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
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message == "not_found");
        }
    }
}
