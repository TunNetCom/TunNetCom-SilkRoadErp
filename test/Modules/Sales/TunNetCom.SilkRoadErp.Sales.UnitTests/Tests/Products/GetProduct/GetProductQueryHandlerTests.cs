//namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products;

//public class GetProductQueryHandlerTests
//{
//    private readonly SalesContext _context;
//    private readonly TestLogger<GetProductQueryHandler> _testLogger;
//    private readonly GetProductQueryHandler _getProductQueryHandler;

//    public GetProductQueryHandlerTests()
//    {
//        var options = new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(databaseName: "SalesContext")
//            .Options;

//        _context = new SalesContext(options);
//        _testLogger = new TestLogger<GetProductQueryHandler>();
//        _getProductQueryHandler = new GetProductQueryHandler(_context, _testLogger);
//    }

    
//    [Fact]
//    public async Task Handle_PaginationRequest_LogsPagination()
//    {
//        // Arrange
//        var query = new GetProductQuery(
//            PageNumber: 1,
//            PageSize: 10,
//            SearchKeyword: null
//        );

//        // Act
//        var result = await _getProductQueryHandler.Handle(query, CancellationToken.None);

//        // Assert
//        Assert.Contains(
//            _testLogger.Logs,
//            log => log.Contains($"Fetching {nameof(Produit)} with pageIndex: {query.PageNumber} and pageSize: {query.PageSize}"));
//        Assert.NotNull(result);
//    }


//    [Fact]
//    public async Task Handle_SearchKeyword_FiltersProducts()
//    {
//        // Arrange
//        var product = Produit.CreateProduct
//            (
//            refe: "RefeNewToFind",
//            nom: "Product testb",
//            qte: 23,
//            qteLimite: 22,
//            remise: 20,
//            remiseAchat: 5,
//            tva: 10,
//            prix: 56,
//            prixAchat: 535,
//            visibilite: true);

//        _context.Produit.Add(product);
//        await _context.SaveChangesAsync();

//        var query = new GetProductQuery(
//           PageNumber: 1,
//           PageSize: 10,
//           SearchKeyword: "Product testb"
//       );

//        // Act
//        var result = await _getProductQueryHandler.Handle(query, CancellationToken.None);

//        // Assert
//        Assert.Single(result);
//        Assert.Equal("Product testb", result.First().Name);
//    }

//    [Fact]
//    public async Task Handle_EmptySearchKeyword_ReturnsAllProducts()
//    {
//        // Arrange
//        var Product1 = Produit.CreateProduct
//            (
//            refe: "Refe111",
//            nom: "test Product",
//            qte: 23,
//            qteLimite: 22,
//            remise: 20,
//            remiseAchat: 5,
//            tva: 10,
//            prix: 56,
//            prixAchat: 535,
//            visibilite: true);

//         var Product2 = Produit.CreateProduct
//            (
//            refe: "Refe222",
//            nom: "test Product",
//            qte: 23,
//            qteLimite: 22,
//            remise: 20,
//            remiseAchat: 5,
//            tva: 10,
//            prix: 56,
//            prixAchat: 535,
//            visibilite: true);

//        var Product3 = Produit.CreateProduct
//            (
//             refe: "Refe333",
//            nom: "test Product",
//            qte: 23,
//            qteLimite: 22,
//            remise: 20,
//            remiseAchat: 5,
//            tva: 10,
//            prix: 56,
//            prixAchat: 535,
//            visibilite: true);

//        _context.Produit.AddRange(Product1, Product2, Product3);
//        await _context.SaveChangesAsync();

//        var query = new GetProductQuery(
//            PageNumber: 1,
//            PageSize: 10,
//            SearchKeyword: ""
//        );

//        // Act
//        var result = await _getProductQueryHandler.Handle(query, CancellationToken.None);

//        // Assert
//        //TODO : Change the logic
//        Assert.True(result.Count >= 3);
//    }
//}
