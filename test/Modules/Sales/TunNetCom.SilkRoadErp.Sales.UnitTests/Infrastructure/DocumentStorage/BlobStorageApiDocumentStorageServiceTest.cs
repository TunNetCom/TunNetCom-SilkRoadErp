using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.DocumentStorage;

public class BlobStorageApiDocumentStorageServiceTest
{
    private const string BaseUrl = "http://localhost:5000";
    private const string Bucket = "silk-road-erp";
    private const string Folder = "documents";
    private static readonly string ValidKey = $"{Folder}/0123456789abcdef0123456789abcdef/paiement.pdf";

    private static BlobStorageApiDocumentStorageService BuildService(
        FakeHttpMessageHandler handler,
        string? folder = Folder,
        string? bucket = Bucket)
    {
        var options = new BlobStorageApiOptions
        {
            Type = "BlobStorageApi",
            BaseUrl = BaseUrl,
            Bucket = bucket ?? string.Empty,
            Folder = folder,
        };

        var client = new HttpClient(handler);
        return new BlobStorageApiDocumentStorageService(
            client,
            Options.Create(options),
            NullLogger<BlobStorageApiDocumentStorageService>.Instance);
    }

    [Fact]
    public async Task SaveAsync_UploadsMultipartForm_AndReturnsKey()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidKey),
        });
        var service = BuildService(handler);

        // Act
        var result = await service.SaveAsync(new byte[] { 1, 2, 3 }, "paiement.pdf");

        // Assert
        _ = result.Should().Be(ValidKey);
        var request = handler.Requests.Should().ContainSingle().Subject;
        _ = request.Method.Should().Be(HttpMethod.Post);
        _ = request.RequestUri.ToString().Should().Be($"{BaseUrl}/storage/{Bucket}");
        var body = handler.RequestBodies.Should().ContainSingle().Subject;
        _ = body.Should().Contain("name=file");
        _ = body.Should().Contain("filename=paiement.pdf");
        _ = body.Should().Contain("name=folder");
        _ = body.Should().Contain(Folder);
    }

    [Fact]
    public async Task SaveAsync_WithoutConfiguredFolder_OmitsFolderField()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ValidKey),
        });
        var service = BuildService(handler, folder: null);

        // Act
        _ = await service.SaveAsync(new byte[] { 1, 2, 3 }, "paiement.pdf");

        // Assert
        _ = handler.RequestBodies.Should().ContainSingle(b => !b.Contains("name=folder"));
    }

    [Fact]
    public async Task SaveAsync_EmptyContent_ThrowsArgumentException()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var service = BuildService(handler);

        // Act
        Func<Task> act = () => service.SaveAsync(Array.Empty<byte>(), "paiement.pdf");

        // Assert
        _ = (await act.Should().ThrowAsync<ArgumentException>()).Which.ParamName.Should().Be("content");
        _ = handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_NonSuccessStatus_ThrowsHttpRequestException()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NotFound));
        var service = BuildService(handler);

        // Act
        Func<Task> act = () => service.SaveAsync(new byte[] { 1, 2, 3 }, "paiement.pdf");

        // Assert
        var exception = await act.Should().ThrowAsync<HttpRequestException>();
        _ = exception.And.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SaveAsync_MissingBucket_ThrowsInvalidOperationException()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var service = BuildService(handler, bucket: null);

        // Act
        Func<Task> act = () => service.SaveAsync(new byte[] { 1, 2, 3 }, "paiement.pdf");

        // Assert
        _ = (await act.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().Contain("Bucket");
        _ = handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_StorageKey_DownloadsBytesFromApi()
    {
        // Arrange
        var expectedBytes = new byte[] { 9, 9, 9 };
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(expectedBytes),
        });
        var service = BuildService(handler);

        // Act
        var result = await service.GetAsync(ValidKey);

        // Assert
        _ = result.Should().BeEquivalentTo(expectedBytes);
        var request = handler.Requests.Should().ContainSingle().Subject;
        _ = request.Method.Should().Be(HttpMethod.Get);
        _ = request.RequestUri.ToString().Should().Be($"{BaseUrl}/storage/{Bucket}/{ValidKey}");
    }

    [Fact]
    public async Task GetAsync_LegacyBase64_DecodesLocallyWithoutHttpCall()
    {
        // Arrange
        var expectedBytes = new byte[] { 1, 2, 3 };
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var service = BuildService(handler);

        // Act
        var result = await service.GetAsync(Convert.ToBase64String(expectedBytes));

        // Assert
        _ = result.Should().BeEquivalentTo(expectedBytes);
        _ = handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_LegacyDataUri_DecodesLocallyWithoutHttpCall()
    {
        // Arrange
        var expectedBytes = Encoding.UTF8.GetBytes("hello");
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var service = BuildService(handler);
        var dataUri = $"data:application/pdf;base64,{Convert.ToBase64String(expectedBytes)}";

        // Act
        var result = await service.GetAsync(dataUri);

        // Assert
        _ = result.Should().BeEquivalentTo(expectedBytes);
        _ = handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_NonSuccessStatus_ThrowsHttpRequestException()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NotFound));
        var service = BuildService(handler);

        // Act
        Func<Task> act = () => service.GetAsync(ValidKey);

        // Assert
        var exception = await act.Should().ThrowAsync<HttpRequestException>();
        _ = exception.And.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_StorageKey_SendsDeleteRequest()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NoContent));
        var service = BuildService(handler);

        // Act
        await service.DeleteAsync(ValidKey);

        // Assert
        var request = handler.Requests.Should().ContainSingle().Subject;
        _ = request.Method.Should().Be(HttpMethod.Delete);
        _ = request.RequestUri.ToString().Should().Be($"{BaseUrl}/storage/{Bucket}/{ValidKey}");
    }

    [Fact]
    public async Task DeleteAsync_LegacyBase64_IsNoOpWithoutHttpCall()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NoContent));
        var service = BuildService(handler);

        // Act
        await service.DeleteAsync(Convert.ToBase64String(new byte[] { 1, 2, 3 }));

        // Assert
        _ = handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_NonSuccessStatus_ThrowsHttpRequestException()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NotFound));
        var service = BuildService(handler);

        // Act
        Func<Task> act = () => service.DeleteAsync(ValidKey);

        // Assert
        var exception = await act.Should().ThrowAsync<HttpRequestException>();
        _ = exception.And.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new();

        public List<HttpRequestMessage> Requests { get; } = new();

        public List<string> RequestBodies { get; } = new();

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _responses.Enqueue(response);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            if (request.Content != null)
            {
                RequestBodies.Add(await request.Content.ReadAsStringAsync(cancellationToken));
            }

            return _responses.Dequeue();
        }
    }
}
