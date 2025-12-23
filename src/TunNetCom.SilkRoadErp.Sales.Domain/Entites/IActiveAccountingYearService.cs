namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Service pour récupérer l'exercice comptable actif.
/// Cette interface est définie dans le projet Domain pour éviter les dépendances circulaires.
/// L'implémentation se trouve dans le projet Api.
/// </summary>
public interface IActiveAccountingYearService
{
    /// <summary>
    /// Récupère l'ID de l'exercice comptable actif de manière asynchrone.
    /// </summary>
    /// <returns>L'ID de l'exercice comptable actif, ou null si aucun exercice n'est actif.</returns>
    Task<int?> GetActiveAccountingYearIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère l'ID de l'exercice comptable actif de manière synchrone.
    /// Utilise la valeur mise en cache si disponible, sinon retourne null.
    /// Cette méthode est utilisée par les Global Query Filters qui ne supportent pas async.
    /// </summary>
    /// <returns>L'ID de l'exercice comptable actif, ou null si non disponible.</returns>
    int? GetActiveAccountingYearId();

    /// <summary>
    /// Invalide le cache de l'exercice comptable actif.
    /// Cette méthode doit être appelée après avoir changé l'exercice comptable actif
    /// pour forcer le rechargement de la nouvelle valeur à la prochaine requête.
    /// </summary>
    void InvalidateCache();

    /// <summary>
    /// Met à jour directement le cache avec une nouvelle valeur d'année comptable.
    /// Cette méthode est utilisée pour mettre à jour immédiatement le cache après un changement d'année.
    /// </summary>
    /// <param name="accountingYearId">L'ID de la nouvelle année comptable active.</param>
    void SetActiveAccountingYearId(int? accountingYearId);
}

