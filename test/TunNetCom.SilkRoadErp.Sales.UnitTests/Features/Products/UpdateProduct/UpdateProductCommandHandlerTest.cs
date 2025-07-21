public class UpdateProductCommandHandlerTest
{
    private readonly SalesContext _context;
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;
    private readonly UpdateProductCommandHandler _handler;
    public UpdateProductCommandHandlerTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase("UpdateProductDb")
            .Options;
        _context = new SalesContext(options);
        _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();
        _handler = new UpdateProductCommandHandler(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProductNotFound()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Refe: "NOT_EXIST",
            Nom: "Nom",
            Qte: 10,
            QteLimite: 5,
            Remise: 0,
            RemiseAchat: 0,
            Tva: 19,
            Prix: 100m,
            PrixAchat: 80m,
            Visibilite: true);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "not_found");
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var product = Produit.CreateProduct(
            refe: "P001",
            nom: "Ancien Nom",
            qte: 5,
            qteLimite: 2,
            remise: 1,
            remiseAchat: 1,
            tva: 19,
            prix: 50m,
            prixAchat: 40m,
            visibilite: true);
        await _context.Produit.AddAsync(product);
        await _context.SaveChangesAsync();
        var command = new UpdateProductCommand(
            Refe: "P001",
            Nom: "Nouveau Nom",
            Qte: 10,
            QteLimite: 5,
            Remise: 5,
            RemiseAchat: 3,
            Tva: 20,
            Prix: 100m,
            PrixAchat: 80m,
            Visibilite: false);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);

        var updatedProduct = await _context.Produit.FindAsync("P001");
        Assert.NotNull(updatedProduct);
        Assert.Equal("Nouveau Nom", updatedProduct!.Nom);
        Assert.Equal(10, updatedProduct.Qte);
        Assert.Equal(5, updatedProduct.QteLimite);
        Assert.Equal(5, updatedProduct.Remise);
        Assert.Equal(3, updatedProduct.RemiseAchat);
        Assert.Equal(20, updatedProduct.Tva);
        Assert.Equal(100m, updatedProduct.Prix);
        Assert.Equal(80m, updatedProduct.PrixAchat);
        Assert.False(updatedProduct.Visibilite);
    }
}
