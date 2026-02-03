using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;

public interface ISoldesApiClient
{
    Task<Result<SoldeClientResponse>> GetSoldeClientAsync(
        int clientId,
        int? accountingYearId,
        CancellationToken cancellationToken);

    Task<Result<SoldeFournisseurResponse>> GetSoldeFournisseurAsync(
        int fournisseurId,
        int? accountingYearId,
        CancellationToken cancellationToken);

    Task<PagedList<ClientSoldeProblemeResponse>> GetClientsAvecProblemesSoldeAsync(
        int pageNumber,
        int pageSize,
        int? accountingYearId = null,
        CancellationToken cancellationToken = default);

    Task<(byte[] Content, string FileName)> ExportClientsAvecProblemesSoldeToPdfAsync(
        int? accountingYearId = null,
        CancellationToken cancellationToken = default);

    Task<(byte[] Content, string FileName)> ExportClientsAvecProblemesSoldeToExcelAsync(
        int? accountingYearId = null,
        CancellationToken cancellationToken = default);

    Task<Result<RestesALivrerParClientResponse>> GetRestesALivrerParClientAsync(
        int? accountingYearId,
        CancellationToken cancellationToken);

    Task<Result<SoldeTiersDepenseResponse>> GetSoldeTiersDepenseAsync(
        int tiersDepenseFonctionnementId,
        int? accountingYearId,
        CancellationToken cancellationToken);
}


