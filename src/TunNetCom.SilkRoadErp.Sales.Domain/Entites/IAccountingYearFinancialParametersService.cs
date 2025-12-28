namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Service pour récupérer les paramètres financiers de l'exercice comptable actif.
/// Cette interface est définie dans le projet Domain pour éviter les dépendances circulaires.
/// L'implémentation se trouve dans le projet Api.
/// </summary>
public interface IAccountingYearFinancialParametersService
{
    /// <summary>
    /// Récupère le timbre fiscal de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le timbre n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le timbre fiscal, ou la valeur de repli.</returns>
    Task<decimal> GetTimbreAsync(decimal fallbackValue = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le taux FODEC de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le taux n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le taux FODEC, ou la valeur de repli.</returns>
    Task<decimal> GetPourcentageFodecAsync(decimal fallbackValue = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le taux de retenue à la source de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le taux n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le taux de retenue à la source, ou la valeur de repli.</returns>
    Task<double> GetPourcentageRetenuAsync(double fallbackValue = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le taux de TVA 0% de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le taux n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le taux de TVA 0%, ou la valeur de repli.</returns>
    Task<decimal> GetVatRate0Async(decimal fallbackValue = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le taux de TVA 7% de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le taux n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le taux de TVA 7%, ou la valeur de repli.</returns>
    Task<decimal> GetVatRate7Async(decimal fallbackValue = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le taux de TVA 13% de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le taux n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le taux de TVA 13%, ou la valeur de repli.</returns>
    Task<decimal> GetVatRate13Async(decimal fallbackValue = 13, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le taux de TVA 19% de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le taux n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le taux de TVA 19%, ou la valeur de repli.</returns>
    Task<decimal> GetVatRate19Async(decimal fallbackValue = 19, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le seuil de retenue à la source de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le seuil n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le seuil de retenue à la source, ou la valeur de repli.</returns>
    Task<decimal> GetSeuilRetenueSourceAsync(decimal fallbackValue = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le nombre de décimales de l'exercice comptable actif.
    /// </summary>
    /// <param name="fallbackValue">Valeur de repli si l'année comptable n'est pas trouvée ou si le nombre n'est pas défini.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le nombre de décimales, ou la valeur de repli.</returns>
    Task<int> GetDecimalPlacesAsync(int fallbackValue = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les paramètres financiers de l'exercice comptable actif en une seule requête.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Un objet contenant tous les paramètres financiers, ou null si l'année comptable n'est pas trouvée.</returns>
    Task<AccountingYearFinancialParameters?> GetAllFinancialParametersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache des paramètres financiers.
    /// Cette méthode doit être appelée après avoir modifié l'année comptable active
    /// pour forcer le rechargement des paramètres à la prochaine requête.
    /// </summary>
    void InvalidateCache();
}

    /// <summary>
    /// Classe contenant tous les paramètres financiers d'une année comptable.
    /// </summary>
public class AccountingYearFinancialParameters
{
    public decimal Timbre { get; set; }
    public decimal PourcentageFodec { get; set; }
    public double PourcentageRetenu { get; set; }
    public decimal VatRate0 { get; set; }
    public decimal VatRate7 { get; set; }
    public decimal VatRate13 { get; set; }
    public decimal VatRate19 { get; set; }
    public decimal VatAmount { get; set; }
    public decimal SeuilRetenueSource { get; set; }
    public int DecimalPlaces { get; set; }
}

