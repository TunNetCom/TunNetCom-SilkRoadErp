namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Service pour vérifier les permissions de l'utilisateur
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Vérifie si l'utilisateur a une permission spécifique
    /// </summary>
    Task<bool> HasPermissionAsync(string permission);

    /// <summary>
    /// Vérifie si l'utilisateur a au moins une des permissions spécifiées
    /// </summary>
    Task<bool> HasAnyPermissionAsync(params string[] permissions);

    /// <summary>
    /// Vérifie si l'utilisateur a toutes les permissions spécifiées
    /// </summary>
    Task<bool> HasAllPermissionsAsync(params string[] permissions);

    /// <summary>
    /// Récupère toutes les permissions de l'utilisateur
    /// </summary>
    Task<IReadOnlyList<string>> GetUserPermissionsAsync();

    /// <summary>
    /// Rafraîchit le cache des permissions
    /// </summary>
    Task RefreshPermissionsAsync();
}

