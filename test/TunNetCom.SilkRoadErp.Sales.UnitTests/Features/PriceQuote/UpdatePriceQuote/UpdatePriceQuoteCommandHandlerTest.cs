//using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote;
//public class UpdatePriceQuoteCommandHandlerTest
//{
//    private DbContextOptions<SalesContext> GetDbOptions(string dbName)
//    {
//        return new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(databaseName: dbName)
//            .Options;
//    }
//    [Fact]
//    public async Task Handle_ShouldReturnFail_WhenQuotationNotFound()
//    {
//        // Arrange
//        var options = GetDbOptions("QuotationNotFoundDb");
//        using var context = new SalesContext(options);
//        var loggerMock = new Mock<ILogger<UpdatePriceQuoteCommandHandler>>();
//        var handler = new UpdatePriceQuoteCommandHandler(context, loggerMock.Object);
//        var command = new UpdatePriceQuoteCommand(
//            Num: 1,
//            IdClient: 100,
//            Date: DateTime.Now,
//            TotHTva: 100,
//            TotTva: 20,
//            TotTtc: 120
//        );
//        // Act
//        var result = await handler.Handle(command, CancellationToken.None);
//        // Assert
//        Assert.True(result.IsFailed);
//        Assert.Equal("not_found", result.Errors[0].Message);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnOk_WhenQuotationIsUpdatedSuccessfully()
//    {
//        // Arrange
//        var options = GetDbOptions("UpdateQuotationSuccessDb");
//        using var context = new SalesContext(options);
//        var loggerMock = new Mock<ILogger<UpdatePriceQuoteCommandHandler>>();
//        var existingDevis = new Devis
//        {
//            Num = 1,
//            IdClient = 10,
//            Date = new DateTime(2024, 1, 1),
//            TotHTva = 50,
//            TotTva = 10,
//            TotTtc = 60
//        };
//        _ = context.Devis.Add(existingDevis);
//        _ = await context.SaveChangesAsync();
//        var handler = new UpdatePriceQuoteCommandHandler(context, loggerMock.Object);
//        var command = new UpdatePriceQuoteCommand(
//            Num: 1,
//            IdClient: 200,
//            Date: DateTime.Now,
//            TotHTva: 500,
//            TotTva: 100,
//            TotTtc: 600
//        );
//        // Act
//        var result = await handler.Handle(command, CancellationToken.None);
//        // Assert
//        Assert.True(result.IsSuccess);
//        var updatedDevis = await context.Devis.FindAsync(1);
//        Assert.Equal(200, updatedDevis.IdClient);
//        Assert.Equal(500, updatedDevis.TotHTva);
//        Assert.Equal(100, updatedDevis.TotTva);
//        Assert.Equal(600, updatedDevis.TotTtc);
//    }
//}
