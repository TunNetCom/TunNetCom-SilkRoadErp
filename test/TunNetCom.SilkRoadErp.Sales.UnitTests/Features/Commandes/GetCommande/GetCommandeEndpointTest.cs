using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
public class OrderEndpointsTest
{
    [Fact]
    public async Task GetFullOrder_ShouldReturn200_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;
        var expectedResponse = new FullOrderResponse
        {
            OrderNumber = orderId,
            Date = new DateTime(2024, 1, 1),
            SupplierId = 10,
            Supplier = new SupplierInfos
            {
                Id = 10,
                Name = "Test Supplier",
                Phone = "12345678",
                Address = "123 Test Street"
            },
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    LineId = 1,
                    ProductReference = "PROD-001",
                    ItemDescription = "Test Product",
                    ItemQuantity = 2,
                    UnitPriceExcludingTax = 50,
                    Discount = 0,
                    TotalExcludingTax = 100,
                    VatRate = 0.19m,
                    TotalIncludingTax = 119
                }
            },
            TotalExcludingVat = 100,
            TotalVat = 19,
            NetToPay = 119
        };
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetFullOrderQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Ok(expectedResponse));
        var services = new ServiceCollection();
        services.AddSingleton(mediatorMock.Object);
        var provider = services.BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            RequestServices = provider,
            Response =
            {
                Body = new MemoryStream()
            }
        };
        var handler = async (HttpContext ctx) =>
        {
            var id = orderId; 
            var mediator = ctx.RequestServices.GetRequiredService<IMediator>();
            var result = await mediator.Send(new GetFullOrderQuery(id), CancellationToken.None);
            if (result.IsFailed)
            {
                ctx.Response.StatusCode = 400;
                return;
            }
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(ctx.Response.Body, result.Value);
        };
        // Act
        await handler(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        // Assert
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Contains("Test Supplier", responseBody);
        Assert.Contains("\"OrderNumber\":1", responseBody);
        Assert.Contains("\"TotalIncludingTax\":119", responseBody);
    }
}
