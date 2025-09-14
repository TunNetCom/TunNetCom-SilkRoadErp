public class DeleteProviderCommandHandlerTest
{
    private readonly Mock<ILogger<DeleteProviderCommandHandler>> _loggerMock;

    public DeleteProviderCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<DeleteProviderCommandHandler>>();
        // Important pour que les appels à Log soient bien enregistrés dans les tests
        _ = _loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
   }
    private SalesContext CreateContextWithData(params Fournisseur[] providers)
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "DeleteProviderTestsDb_" + System.Guid.NewGuid())
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
    public async Task Handle_ShouldDeleteProvider_WhenProviderExists()
    {
        // Arrange
        var provider = new Fournisseur { Id = 1, Nom = "Provider1", Tel = "1234567890" };
        using var context = CreateContextWithData(provider);
        var handler = new DeleteProviderCommandHandler(context, _loggerMock.Object);
        var command = new DeleteProviderCommand(provider.Id);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(await context.Fournisseur.FindAsync(provider.Id));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("deleted")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProviderDoesNotExist()
    {
        // Arrange
        using var context = CreateContextWithData(); // no providers
        var handler = new DeleteProviderCommandHandler(context, _loggerMock.Object);
        var command = new DeleteProviderCommand(999); // non-existent ID
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message.Contains("not_found"));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("not found") || v.ToString().Contains("not_found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }
}
