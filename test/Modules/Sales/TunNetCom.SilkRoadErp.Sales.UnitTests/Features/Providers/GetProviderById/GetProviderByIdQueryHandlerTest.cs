public class GetProviderByIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetProviderByIdQueryHandler>> _loggerMock;
    public GetProviderByIdQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetProviderByIdQueryHandler>>();
        _ = _loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _ = _loggerMock.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()))
        .Callback<LogLevel, EventId, object, Exception, Delegate>((level, eventId, state, ex, func) =>
        {
            var message = func.DynamicInvoke(state, ex) as string;
            Console.WriteLine($"LogLevel: {level}, Message: {message}");
        });
    }
    private SalesContext CreateContextWithData(params Fournisseur[] providers)
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "GetProviderByIdTestsDb_" + Guid.NewGuid())
            .Options;
        var context = new SalesContext(options);
        if (providers.Length > 0)
        {
            context.Fournisseur.AddRange(providers);
            _ = context.SaveChanges();
        }
        return context;
    }

    [Fact]
    public async Task Handle_ShouldReturnProvider_WhenProviderExists()
    {
        var provider = new Fournisseur
        {
            Id = 1,
            Nom = "Test Provider",
            Tel = "1234567890",
            Fax = "Fax",
            Matricule = "Matricule",
            Code = "Code",
            CodeCat = "CodeCat",
            EtbSec = "EtbSec",
            Mail = "mail@example.com",
            MailDeux = "mail2@example.com",
            Constructeur = true,
            Adresse = "Address"
        };
        using var context = CreateContextWithData(provider);
        var handler = new GetProviderByIdQueryHandler(context, _loggerMock.Object);
        var query = new GetProviderByIdQuery(provider.Id);
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(provider.Id, result.Value.Id);
        Assert.Equal(provider.Nom, result.Value.Nom);
        _loggerMock.Verify(
     x => x.Log(
         LogLevel.Information,
         It.IsAny<EventId>(),
         It.Is<It.IsAnyType>((v, _) => v.ToString().IndexOf("fetching", StringComparison.OrdinalIgnoreCase) >= 0),
         null,
         It.IsAny<Func<It.IsAnyType, Exception, string>>()),
     Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().IndexOf("fetched", StringComparison.OrdinalIgnoreCase) >= 0),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProviderDoesNotExist()
    {
        using var context = CreateContextWithData(); // aucun fournisseur
        var handler = new GetProviderByIdQueryHandler(context, _loggerMock.Object);
        var query = new GetProviderByIdQuery(999); // ID inexistant
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message.Contains("not_found"));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
