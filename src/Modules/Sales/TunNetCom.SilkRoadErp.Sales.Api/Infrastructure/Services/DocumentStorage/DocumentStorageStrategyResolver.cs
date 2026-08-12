namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Strategy pattern : reçoit toutes les implémentations de <see cref="IDocumentStorageService"/>
/// et délègue la demande à celle dont le <see cref="IDocumentStorageService.Type"/> correspond
/// au type demandé. Chaque nouveau fournisseur s'ajoute sans modifier cette classe.
/// </summary>
public class DocumentStorageStrategyResolver
{
    private readonly Dictionary<string, IDocumentStorageService> _strategies;

    public DocumentStorageStrategyResolver(IEnumerable<IDocumentStorageService> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Type, s => s);
    }

    public Task<string> SaveAsync(byte[] content, string storageType, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (!_strategies.TryGetValue(storageType, out var strategy))
        {
            throw new ArgumentException(
                "Unknown document storage type",
                nameof(storageType));
        }

        return strategy.SaveAsync(content, fileName, cancellationToken);
    }

    public Task<byte[]> GetAsync(string path, string storageType, CancellationToken cancellationToken = default)
    {
        if (!_strategies.TryGetValue(storageType, out var strategy))
        {
            throw new ArgumentException(
                "Unknown document storage type",
                nameof(storageType));
        }

        return strategy.GetAsync(path, cancellationToken);
    }

    public Task DeleteAsync(string path, string storageType, CancellationToken cancellationToken = default)
    {
        if (!_strategies.TryGetValue(storageType, out var strategy))
        {
            throw new ArgumentException(
                "Unknown document storage type",
                nameof(storageType));
        }

        return strategy.DeleteAsync(path, cancellationToken);
    }
}