public class GetProviderQueryHandlerTest
{
    private readonly Mock<ILogger<GetProviderQueryHandler>> _loggerMock;

    public GetProviderQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetProviderQueryHandler>>();
        _loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
    }
    private SalesContext CreateContextWithData(params Fournisseur[] providers)
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "GetProviderTestsDb_" + System.Guid.NewGuid())
            .Options;
        var context = new SalesContext(options);
        if (providers.Length > 0)
        {
            context.Fournisseur.AddRange(providers);
            context.SaveChanges();
       }
        return context;
    }
    [Fact]
    public async Task Handle_ShouldReturnPagedProviders()
    {
        // Arrange
        var providers = new List<Fournisseur>
    {
        new Fournisseur { Id = 1, Nom = "Alpha", Tel = "1111111111" },
        new Fournisseur { Id = 2, Nom = "Beta", Tel = "2222222222" },
        new Fournisseur { Id = 3, Nom = "Gamma", Tel = "3333333333" }
    };
        using var context = CreateContextWithData(providers.ToArray());
        var handler = new GetProviderQueryHandler(context, _loggerMock.Object);
        var query = new GetProviderQuery(
            PageNumber: 1,
            PageSize: 2,
            SearchKeyword: null);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count); 
        Assert.Equal(3, result.TotalCount);
        Assert.Contains(result.Items, p => p.Nom == "Alpha");
        Assert.Contains(result.Items, p => p.Nom == "Beta");

        _loggerMock.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Fetching Fournisseur with pageIndex")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        _loggerMock.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Fetched")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchKeyword()
    {
        // Arrange
        var providers = new List<Fournisseur>
        {
            new Fournisseur { Id = 1, Nom = "Alpha", Tel = "1111111111" },
            new Fournisseur { Id = 2, Nom = "Beta", Tel = "2222222222" },
            new Fournisseur { Id = 3, Nom = "Gamma", Tel = "3333333333" }
        };
        using var context = CreateContextWithData(providers.ToArray());
        var handler = new GetProviderQueryHandler(context, _loggerMock.Object);
        var query = new GetProviderQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchKeyword: "Beta");
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Beta", result.Items.First().Nom);
    }
}
