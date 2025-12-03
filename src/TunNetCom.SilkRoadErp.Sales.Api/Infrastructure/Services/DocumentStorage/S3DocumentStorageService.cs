namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Implémentation du service de stockage utilisant AWS S3
/// À implémenter dans le futur si nécessaire
/// </summary>
public class S3DocumentStorageService : IDocumentStorageService
{
    private readonly ILogger<S3DocumentStorageService> _logger;

    public S3DocumentStorageService(ILogger<S3DocumentStorageService> logger)
    {
        _logger = logger;
    }

    public Task<string> SaveAsync(byte[] content, string? fileName = null, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter l'upload vers S3
        _logger.LogWarning("S3DocumentStorageService.SaveAsync not implemented yet");
        throw new NotImplementedException("S3 storage not implemented yet");
    }

    public Task<byte[]> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter le téléchargement depuis S3
        _logger.LogWarning("S3DocumentStorageService.GetAsync not implemented yet");
        throw new NotImplementedException("S3 storage not implemented yet");
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter la suppression depuis S3
        _logger.LogWarning("S3DocumentStorageService.DeleteAsync not implemented yet");
        throw new NotImplementedException("S3 storage not implemented yet");
    }
}


