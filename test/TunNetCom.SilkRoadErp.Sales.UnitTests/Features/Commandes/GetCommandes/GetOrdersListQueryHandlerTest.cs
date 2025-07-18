using TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
public class GetOrdersListQueryHandlerTest
{
    private SalesContext CreateDbContextWithData()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "OrdersTestDb_" + Guid.NewGuid())
            .Options;
        var context = new SalesContext(options);
        var order1 = new Commandes
        {
            Num = 1,
            Date = new DateTime(2024, 1, 1),
            FournisseurId = 10,
            LigneCommandes = new List<LigneCommandes>
            {
                new LigneCommandes { RefProduit = "P1", TotHt = 100, TotTtc = 119 },
                new LigneCommandes { RefProduit = "P2", TotHt = 50, TotTtc = 59.5m }
            }
        };
        context.Commandes.Add(order1);
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoOrdersExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase("EmptyOrdersDb_" + Guid.NewGuid())
            .Options;
        var context = new SalesContext(options);
        var loggerMock = new Mock<ILogger<GetOrdersListQueryHandler>>();
        var handler = new GetOrdersListQueryHandler(context, loggerMock.Object);
        // Act
        var result = await handler.Handle(new GetOrdersListQuery(), CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        loggerMock.Verify(
           x => x.Log(
               LogLevel.Information,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No orders found in the database.")),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrdersList_WhenOrdersExist()
    {
        // Arrange
        var context = CreateDbContextWithData();
        var loggerMock = new Mock<ILogger<GetOrdersListQueryHandler>>();
        var handler = new GetOrdersListQueryHandler(context, loggerMock.Object);
        // Act
        var result = await handler.Handle(new GetOrdersListQuery(), CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        var orders = result.Value;
        Assert.Single(orders);
        var order = orders.First();
        Assert.Equal(1, order.OrderNumber);
        Assert.Equal(10, order.SupplierId);
        Assert.Equal(150, order.TotalExcludingVat);
        Assert.Equal(178.5m, order.NetToPay);
        Assert.Equal(28.5m, order.TotalVat);
    }
}
