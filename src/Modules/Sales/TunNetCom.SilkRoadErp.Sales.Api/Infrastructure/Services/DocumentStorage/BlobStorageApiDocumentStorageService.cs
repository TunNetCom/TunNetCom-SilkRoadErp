using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Implémentation du service de stockage utilisant l'API BlobStorage (MinIO).
/// Les documents sont envoyés sur les endpoints upload/download/delete de l'API.
/// Les anciennes valeurs stockées en Base64 dans la base de données restent lisibles
/// (aucune migration requise, elles sont décodées localement).
/// </summary>
public class BlobStorageApiDocumentStorageService : IDocumentStorageService
{
    public string Type => "BlobStorageApi";

    private static readonly Regex StorageKeyRegex = new(
        @"^(?:[^/]+/)?[0-9a-f]{32}/[^/]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly HttpClient _httpClient;
    private readonly BlobStorageApiOptions _options;
    private readonly ILogger<BlobStorageApiDocumentStorageService> _logger;

    public BlobStorageApiDocumentStorageService(
        HttpClient httpClient,
        IOptions<BlobStorageApiOptions> options,
        ILogger<BlobStorageApiDocumentStorageService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> SaveAsync(byte[] content, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (content == null || content.Length == 0)
        {
            throw new ArgumentException("Content cannot be null or empty", nameof(content));
        }

        ValidateConfiguration();

        var safeFileName = string.IsNullOrWhiteSpace(fileName) ? $"{Guid.NewGuid():N}.bin" : fileName;
        var url = $"{_options.BaseUrl.TrimEnd('/')}/storage/{Uri.EscapeDataString(_options.Bucket)}";

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "file", safeFileName);

        if (!string.IsNullOrWhiteSpace(_options.Folder))
        {
            form.Add(new StringContent(_options.Folder!), "folder");
        }

        _logger.LogDebug("Uploading document to BlobStorage API. Size: {Size} bytes, FileName: {FileName}, Bucket: {Bucket}", content.Length, safeFileName, _options.Bucket);

        using var response = await _httpClient.PostAsync(url, form, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("BlobStorage API upload failed with status {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
            throw new HttpRequestException(
                $"BlobStorage API upload failed with status {(int)response.StatusCode} {response.ReasonPhrase}. Bucket: {_options.Bucket}",
                null,
                response.StatusCode);
        }

        var key = (await response.Content.ReadAsStringAsync(cancellationToken)).Trim().Trim('"');

        //temporary hardcoded public URL
        var fullpublicFile = "https://s3.aion-time.com/silk-road-erp";
        _logger.LogDebug("Document uploaded to BlobStorage API with key {Key}", key);
        return $"{fullpublicFile}/{key}";
    }

    public async Task<byte[]> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException("Storage path cannot be null or empty", nameof(storagePath));
        }

        // Anciennes valeurs Base64 stockées dans la base : aucun appel HTTP nécessaire
        if (!IsStorageKey(storagePath))
        {
            return ConvertFromLegacyBase64(storagePath);
        }

        ValidateConfiguration();

        var url = BuildObjectUrl(storagePath);
        _logger.LogDebug("Downloading document from BlobStorage API. Key: {Key}", storagePath);

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("BlobStorage API download failed with status {StatusCode} for key {Key}", response.StatusCode, storagePath);
            throw new HttpRequestException(
                $"BlobStorage API download failed with status {(int)response.StatusCode} {response.ReasonPhrase}",
                null,
                response.StatusCode);
        }

        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        _logger.LogDebug("Document downloaded from BlobStorage API. Size: {Size} bytes, Key: {Key}", content.Length, storagePath);
        return content;
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return;
        }

        // Anciennes valeurs Base64 stockées dans la base : le contenu est supprimé avec l'enregistrement
        if (!IsStorageKey(storagePath))
        {
            _logger.LogDebug("Legacy Base64 document detected, deletion handled by database record deletion");
            return;
        }

        ValidateConfiguration();

        var url = BuildObjectUrl(storagePath);
        _logger.LogDebug("Deleting document from BlobStorage API. Key: {Key}", storagePath);

        using var response = await _httpClient.DeleteAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("BlobStorage API delete failed with status {StatusCode} for key {Key}", response.StatusCode, storagePath);
            throw new HttpRequestException(
                $"BlobStorage API delete failed with status {(int)response.StatusCode} {response.ReasonPhrase}",
                null,
                response.StatusCode);
        }
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            throw new InvalidOperationException("DocumentStorage:BaseUrl is not configured");
        }

        if (string.IsNullOrWhiteSpace(_options.Bucket))
        {
            throw new InvalidOperationException("DocumentStorage:Bucket is not configured");
        }
    }

    private string BuildObjectUrl(string storagePath)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var escapedKey = string.Join('/', storagePath.Split('/').Select(Uri.EscapeDataString));
        return $"{baseUrl}/storage/{Uri.EscapeDataString(_options.Bucket)}/{escapedKey}";
    }

    private static bool IsStorageKey(string storagePath)
        => StorageKeyRegex.IsMatch(storagePath);

    private static byte[] ConvertFromLegacyBase64(string storagePath)
    {
        var base64 = storagePath;
        var dataPrefixIndex = storagePath.IndexOf(',');
        if (storagePath.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && dataPrefixIndex >= 0)
        {
            base64 = storagePath[(dataPrefixIndex + 1)..];
        }

        try
        {
            return Convert.FromBase64String(base64);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid Base64 format in storage path", nameof(storagePath), ex);
        }
    }
}
