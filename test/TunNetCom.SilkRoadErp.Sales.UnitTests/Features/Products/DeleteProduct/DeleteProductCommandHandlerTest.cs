namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.Products.DeleteProduct
{
    public class DeleteProductCommandHandlerTest
    {
        private readonly SalesContext _context;
        private readonly Mock<ILogger<DeleteProductCommandHandler>> _loggerMock;
        private readonly DeleteProductCommandHandler _handler;
        public DeleteProductCommandHandlerTest()
        {
            // Configurer DbContext en mémoire
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_DeleteProduct")
                .Options;
            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<DeleteProductCommandHandler>>();
            _handler = new DeleteProductCommandHandler(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenProductNotFound()
        {
            // Arrange
            var command = new DeleteProductCommand("NON_EXISTENT_REF");
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("not_found");
        }

        [Fact]
        public async Task Handle_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var product = Produit.CreateProduct(
                "P001",
                "Produit Test",
                qte: 10,
                qteLimite: 5,
                remise: 0,
                remiseAchat: 0,
                tva: 19,
                prix: 100,
                prixAchat: 80,
                visibilite: true);
            await _context.Produit.AddAsync(product);
            await _context.SaveChangesAsync();
            var command = new DeleteProductCommand("P001");
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            var deletedProduct = await _context.Produit.FindAsync("P001");
            deletedProduct.Should().BeNull();
        }
    }
}
