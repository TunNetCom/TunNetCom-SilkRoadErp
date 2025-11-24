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
}

