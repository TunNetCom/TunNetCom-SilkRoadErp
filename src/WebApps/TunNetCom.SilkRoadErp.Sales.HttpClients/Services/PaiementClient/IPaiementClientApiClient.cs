using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementClient;

public interface IPaiementClientApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreatePaiementClientAsync(
        CreatePaiementClientRequest request,
        CancellationToken cancellationToken);

    Task<Result<PaiementClientResponse>> GetPaiementClientAsync(
        int id,
        CancellationToken cancellationToken);

    Task<PagedList<PaiementClientResponse>> GetPaiementsClientAsync(
        int? clientId,
        int? accountingYearId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Result> UpdatePaiementClientAsync(
        int id,
        UpdatePaiementClientRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeletePaiementClientAsync(
        int id,
        CancellationToken cancellationToken);
}



