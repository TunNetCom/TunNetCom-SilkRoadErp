using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface ISoldeClientCalculationService
{
    /// <summary>
    /// Calculates solde components and balance for a single client in the given accounting year.
    /// Returns null if client or accounting year is invalid.
    /// </summary>
    Task<SoldeClientCalculDto?> CalculateSoldeClientAsync(
        int clientId,
        int accountingYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solde components for all clients that have activity in the given accounting year.
    /// One (or few) bulk query(ies) - no N+1. Used by "Clients avec problème de solde" and restes à livrer.
    /// </summary>
    Task<IReadOnlyList<SoldeClientItemDto>> GetSoldesClientsForAccountingYearAsync(
        int accountingYearId,
        CancellationToken cancellationToken = default);
}
