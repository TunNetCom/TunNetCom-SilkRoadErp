namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

/// <summary>
/// Service générique pour le stockage de documents (PDF, etc.)
/// Permet de switcher entre différentes implémentations (Base64, S3, Azure Blob Storage, etc.)
/// </summary>
public interface IDocumentStorageService
{
    /// <summary>
    /// Sauvegarde un document et retourne le chemin/identifiant de stockage
    /// </summary>
    /// <param name="content">Contenu du document en bytes</param>
    /// <param name="fileName">Nom du fichier (optionnel, pour référence)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Chemin/identifiant du document stocké</returns>
    Task<string> SaveAsync(byte[] content, string? fileName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le contenu d'un document à partir de son chemin/identifiant
    /// </summary>
    /// <param name="storagePath">Chemin/identifiant du document</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Contenu du document en bytes</returns>
    Task<byte[]> GetAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un document à partir de son chemin/identifiant
    /// </summary>
    /// <param name="storagePath">Chemin/identifiant du document</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
}


