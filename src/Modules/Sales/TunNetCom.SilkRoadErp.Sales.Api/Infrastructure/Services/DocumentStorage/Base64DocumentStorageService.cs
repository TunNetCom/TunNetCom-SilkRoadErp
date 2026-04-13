using System.Text;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Implémentation du service de stockage utilisant Base64 dans la base de données
/// Le storagePath retourné est le contenu Base64 lui-même
/// </summary>
public class Base64DocumentStorageService : IDocumentStorageService
{
    private readonly ILogger<Base64DocumentStorageService> _logger;

    public Base64DocumentStorageService(ILogger<Base64DocumentStorageService> logger)
    {
        _logger = logger;
    }

    public Task<string> SaveAsync(byte[] content, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (content == null || content.Length == 0)
        {
            throw new ArgumentException("Content cannot be null or empty", nameof(content));
        }

        try
        {
            var base64Content = Convert.ToBase64String(content);
            _logger.LogDebug("Document saved as Base64. Size: {Size} bytes, FileName: {FileName}", content.Length, fileName);
            return Task.FromResult(base64Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting content to Base64. FileName: {FileName}", fileName);
            throw;
        }
    }

    public Task<byte[]> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException("Storage path cannot be null or empty", nameof(storagePath));
        }

        try
        {
            var content = Convert.FromBase64String(storagePath);
            _logger.LogDebug("Document retrieved from Base64. Size: {Size} bytes", content.Length);
            return Task.FromResult(content);
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Invalid Base64 format in storage path");
            throw new ArgumentException("Invalid Base64 format", nameof(storagePath), ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting Base64 to bytes");
            throw;
        }
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // Pour Base64, la suppression est gérée par la suppression de l'enregistrement en base
        // Pas d'action nécessaire ici car le contenu est dans la base de données
        _logger.LogDebug("Delete requested for Base64 document (no-op, handled by database record deletion)");
        return Task.CompletedTask;
    }
}


