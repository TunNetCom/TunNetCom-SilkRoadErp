using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface ISoldeFournisseurCalculationService
{
    /// <summary>
    /// Calculates solde components and balance for a single fournisseur in the given accounting year.
    /// Returns null if fournisseur or accounting year is invalid.
    /// </summary>
    Task<SoldeFournisseurCalculDto?> CalculateSoldeFournisseurAsync(
        int fournisseurId,
        int accountingYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solde components for all fournisseurs that have activity in the given accounting year.
    /// One (or few) bulk query(ies) - no N+1. Used by "Fournisseurs avec problème de solde".
    /// </summary>
    Task<IReadOnlyList<SoldeFournisseurItemDto>> GetSoldesFournisseursForAccountingYearAsync(
        int accountingYearId,
        CancellationToken cancellationToken = default);
}
