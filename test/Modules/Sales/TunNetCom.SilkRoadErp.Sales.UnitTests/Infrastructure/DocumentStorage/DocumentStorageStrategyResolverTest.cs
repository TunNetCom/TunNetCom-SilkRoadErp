using Moq;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.DocumentStorage;

public class DocumentStorageStrategyResolverTest
{
    private static Mock<IDocumentStorageService> CreateStrategy(string type)
    {
        var mock = new Mock<IDocumentStorageService>();
        mock.SetupGet(s => s.Type).Returns(type);
        return mock;
    }

    private static DocumentStorageStrategyResolver BuildResolver(out Mock<IDocumentStorageService> base64Mock, out Mock<IDocumentStorageService> blobStorageApiMock)
    {
        base64Mock = CreateStrategy("Base64");
        blobStorageApiMock = CreateStrategy("BlobStorageApi");
        var s3Mock = CreateStrategy("S3");

        return new DocumentStorageStrategyResolver(
            new IDocumentStorageService[] { base64Mock.Object, s3Mock.Object, blobStorageApiMock.Object });
    }

    [Fact]
    public async Task SaveAsync_DelegatesToStrategyMatchingStorageType()
    {
        // Arrange
        var resolver = BuildResolver(out var base64Mock, out var blobStorageApiMock);
        var content = new byte[] { 1, 2, 3 };
        base64Mock.Setup(s => s.SaveAsync(content, "file.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync("base64/path");

        // Act
        var result = await resolver.SaveAsync(content, "Base64", "file.jpg");

        // Assert
        result.Should().Be("base64/path");
        blobStorageApiMock.Verify(s => s.SaveAsync(It.IsAny<byte[]>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_DelegatesToStrategyMatchingStorageType()
    {
        // Arrange
        var resolver = BuildResolver(out _, out var blobStorageApiMock);
        blobStorageApiMock.Setup(s => s.GetAsync("path", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[] { 4, 5, 6 });

        // Act
        var result = await resolver.GetAsync("path", "BlobStorageApi");

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 4, 5, 6 });
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToStrategyMatchingStorageType()
    {
        // Arrange
        var resolver = BuildResolver(out _, out var blobStorageApiMock);
        blobStorageApiMock.Setup(s => s.DeleteAsync("path", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await resolver.DeleteAsync("path", "BlobStorageApi");

        // Assert
        blobStorageApiMock.Verify(s => s.DeleteAsync("path", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("")]
    public void SaveAsync_UnknownStorageType_ThrowsArgumentException(string storageType)
    {
        // Arrange
        var resolver = BuildResolver(out _, out _);

        // Act
        Func<Task> act = () => resolver.SaveAsync(new byte[] { 1 }, storageType);

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .Result.Which.ParamName.Should().Be("storageType");
    }

    [Fact]
    public void GetAsync_UnknownStorageType_ThrowsArgumentException()
    {
        // Arrange
        var resolver = BuildResolver(out _, out _);

        // Act
        Func<Task> act = () => resolver.GetAsync("path", "unknown");

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .Result.Which.ParamName.Should().Be("storageType");
    }

    [Fact]
    public void DeleteAsync_UnknownStorageType_ThrowsArgumentException()
    {
        // Arrange
        var resolver = BuildResolver(out _, out _);

        // Act
        Func<Task> act = () => resolver.DeleteAsync("path", "unknown");

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .Result.Which.ParamName.Should().Be("storageType");
    }
}