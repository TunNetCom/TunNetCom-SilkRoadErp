public class CreateProductCommandHandlerTest
{
    private SalesContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new SalesContext(options);
    }
    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenProductDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryContext();
        var logger = new Mock<ILogger<CreateProductCommandHandler>>();
        var handler = new CreateProductCommandHandler(context, logger.Object);
        var command = new CreateProductCommand(
            "REF001", "Product A", 10, 2, 5.0, 3.0, 19.0, 100m, 80m, true
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("REF001", result.Value);
        Assert.Single(context.Produit);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProductReferenceOrNameExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        var existingProduct = Produit.CreateProduct("REF001", "Product A", 5, 1, 0, 0, 19, 50m, 40m, true);
        context.Produit.Add(existingProduct);
        await context.SaveChangesAsync();
        var logger = new Mock<ILogger<CreateProductCommandHandler>>();
        var handler = new CreateProductCommandHandler(context, logger.Object);
        var command = new CreateProductCommand(
            "REF001", "Product A", 10, 2, 5.0, 3.0, 19.0, 100m, 80m, true
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "product_refe_or_name_exist");
    }
}
