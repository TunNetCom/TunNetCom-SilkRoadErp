using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceClient;

public interface IRetenueSourceClientApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateRetenueSourceClientAsync(
        CreateRetenueSourceClientRequest request,
        CancellationToken cancellationToken);

    Task<Result<RetenueSourceClientResponse>> GetRetenueSourceClientAsync(
        int numFacture,
        CancellationToken cancellationToken);

    Task<Result<byte[]>> GetRetenueSourceClientPdfAsync(
        int numFacture,
        CancellationToken cancellationToken);

    Task<Result> UpdateRetenueSourceClientAsync(
        int numFacture,
        UpdateRetenueSourceClientRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeleteRetenueSourceClientAsync(
        int numFacture,
        CancellationToken cancellationToken);
}


