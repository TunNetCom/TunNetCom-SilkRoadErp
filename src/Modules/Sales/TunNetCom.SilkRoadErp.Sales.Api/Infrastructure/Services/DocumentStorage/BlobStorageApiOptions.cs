namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Options de configuration pour le stockage de documents via l'API BlobStorage (MinIO)
/// </summary>
public class BlobStorageApiOptions
{
    public const string SectionName = "DocumentStorage";

    public string Type { get; init; } = "Base64";

    public string BaseUrl { get; init; } = string.Empty;

    public string Bucket { get; init; } = string.Empty;

    public string? Folder { get; init; }

    public double TimeoutSeconds { get; init; } = 100;
}
