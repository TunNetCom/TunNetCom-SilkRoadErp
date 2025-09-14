using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers
{
    public class GetCustomerByIdEndpointTests
    {
        [Fact]
        public async Task HandleGetCustomerByIdAsync_ShouldReturnOk_WhenCustomerFound()
        {
            // Arrange
            var customer = new CustomerResponse
            {
                Id = 1,
                Name = "John Doe",
                Tel = "123456789",
                Adresse = "123 Street",
                Matricule = "MAT123",
                Code = "CODE1",
                CodeCat = "CAT1",
                EtbSec = "ETB1",
                Mail = "john@example.com"
            };
            var mediatorMock = new Mock<IMediator>();
            _ = mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(customer));
            // Act
            var result = await GetCustomerByIdEndpoint.HandleGetCustomerByIdAsync(mediatorMock.Object, 1, CancellationToken.None);
            // Assert
            _ = result.Should().BeOfType<Results<Ok<CustomerResponse>, NotFound>>();
            var okResult = (result as Results<Ok<CustomerResponse>, NotFound>)!.Result as Ok<CustomerResponse>;
            _ = okResult.Should().NotBeNull();
            _ = okResult!.Value.Should().BeEquivalentTo(customer);
        }

        [Fact]
        public async Task HandleGetCustomerByIdAsync_ShouldReturnNotFound_WhenCustomerNotFound()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            _ = mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<CustomerResponse>(EntityNotFound.Error()));
            // Act
            var result = await GetCustomerByIdEndpoint.HandleGetCustomerByIdAsync(mediatorMock.Object, 999, CancellationToken.None);
            // Assert
            _ = result.Should().BeOfType<Results<Ok<CustomerResponse>, NotFound>>();
            var notFoundResult = (result as Results<Ok<CustomerResponse>, NotFound>)!.Result as NotFound;
            _ = notFoundResult.Should().NotBeNull();
        }

    }
}
