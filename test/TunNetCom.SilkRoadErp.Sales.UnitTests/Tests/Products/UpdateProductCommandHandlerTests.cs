﻿namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly SalesContext _context;
    private readonly TestLogger<UpdateProductCommandHandler> _testLogger;
    private readonly UpdateProductCommandHandler _updateProductCommandHandler;

    public UpdateProductCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "SalesContext")
            .Options;
        _context = new SalesContext(options);
        _testLogger = new TestLogger<UpdateProductCommandHandler>();
        _updateProductCommandHandler = new UpdateProductCommandHandler(_context, _testLogger);
    } 

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsFailResult()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Refe: "RefeNotFound",
            Nom: "Update Product",
            Qte: 23,
            QteLimite: 22,
            Remise: 20,
            RemiseAchat: 5,
            Tva: 10,
            Prix: 56,
            PrixAchat: 535,
            Visibilite: true
           );

        // Act
        var result = await _updateProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("not_found", result.Errors.First().Message);
        Assert.Contains(_testLogger.Logs, log => log.Contains($"{nameof(Produit)} with ID: {command.Refe} not found"));
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccessResult()
    {
        // Arrange
        var existingProduct = Produit.CreateProduct(
            refe: "Refe123ToUpate",
            nom: "Existing Product",
            qte: 23,
            qteLimite: 22,
            remise: 20,
            remiseAchat: 5,
            tva: 10,
            prix: 56,
            prixAchat: 535,
            visibilite: true);

        _context.Produit.Add(existingProduct);
        await _context.SaveChangesAsync();

        var command = new UpdateProductCommand(
            Refe: existingProduct.Refe,
            Nom: "Updated Product 123",
            Qte: 23,
            QteLimite: 22,
            Remise: 20,
            RemiseAchat: 5,
            Tva: 10,
            Prix: 56,
            PrixAchat: 535,
            Visibilite: true
           );

        // Act
        var result = await _updateProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(
            _testLogger.Logs,
            log => log.Contains($"{nameof(Produit)} with ID: {existingProduct.Refe} updated successfully"));
    }
}
