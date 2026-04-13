namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Implémentation du service de stockage utilisant Azure Blob Storage
/// À implémenter dans le futur si nécessaire
/// </summary>
public class AzureBlobStorageService : IDocumentStorageService
{
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
    }

    public Task<string> SaveAsync(byte[] content, string? fileName = null, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter l'upload vers Azure Blob Storage
        _logger.LogWarning("AzureBlobStorageService.SaveAsync not implemented yet");
        throw new NotImplementedException("Azure Blob Storage not implemented yet");
    }

    public Task<byte[]> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter le téléchargement depuis Azure Blob Storage
        _logger.LogWarning("AzureBlobStorageService.GetAsync not implemented yet");
        throw new NotImplementedException("Azure Blob Storage not implemented yet");
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter la suppression depuis Azure Blob Storage
        _logger.LogWarning("AzureBlobStorageService.DeleteAsync not implemented yet");
        throw new NotImplementedException("Azure Blob Storage not implemented yet");
    }
}


